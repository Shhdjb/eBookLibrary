using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Threading.Tasks;
using eBookLibrary.Models;
using System.Net;

namespace eBookLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        // Display all books with filters
        public ActionResult Index(string search, string sortBy, string genre, int? ageLimit, bool? onSale, int? yearOfPublishing)
        {
            var books = _context.Books.AsQueryable();

            ViewBag.Genres = _context.Books.Select(b => b.Genre).Distinct().ToList();
            ViewBag.Years = _context.Books.Select(b => b.YearOfPublishing).Distinct().OrderByDescending(y => y).ToList();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(b => b.Title.Contains(search) || b.Author.Contains(search) || b.Publisher.Contains(search));
            }
            if (!string.IsNullOrEmpty(genre))
            {
                books = books.Where(b => b.Genre == genre);
            }
            if (ageLimit.HasValue)
            {
                books = books.Where(b => b.AgeLimit >= ageLimit.Value);
            }
            if (yearOfPublishing.HasValue)
            {
                books = books.Where(b => b.YearOfPublishing == yearOfPublishing.Value);
            }
            if (onSale.HasValue && onSale.Value)
            {
                books = books.Where(b => b.DiscountPrice.HasValue);
            }

            // Sorting
            switch (sortBy)
            {
                case "priceAsc":
                    books = books.OrderBy(b => b.DiscountPrice ?? b.BuyPrice);
                    break;
                case "priceDesc":
                    books = books.OrderByDescending(b => b.DiscountPrice ?? b.BuyPrice);
                    break;
                case "popularity":
                    books = books.OrderByDescending(b => b.Popularity);
                    break;
                case "year":
                    books = books.OrderByDescending(b => b.YearOfPublishing);
                    break;
                default:
                    books = books.OrderBy(b => b.Title);
                    break;
            }

            return View(books.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BuyBookAsync(int bookId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                TempData["Error"] = "You must be logged in to buy a book.";
                return RedirectToAction("Index");
            }

            var book = _context.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("Index");
            }

            if (book.InStock > 0)
            {
                // Decrement the stock count
                book.InStock--;

                // Record the purchase
                var purchase = new Purchase
                {
                    BookId = bookId,
                    UserId = userId,
                    PurchaseDate = DateTime.Now,
                    Price = book.BuyPrice
                };

                _context.Purchases.Add(purchase);
                _context.SaveChanges();

                // Send email notification
                await SendEmailAsync("ranayoun11@gmail.com", "Book Purchase Confirmation", $"You have successfully purchased the book: {book.Title}.");
                TempData["Message"] = "You successfully purchased the book.";
            }
            else
            {
                TempData["Error"] = "This book is out of stock.";
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult BorrowBook(int bookId)
        {
            // Fetch the selected book
            var book = _context.Books.SingleOrDefault(b => b.Id == bookId);

            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("Index");
            }

            // Calculate waiting list count and next availability
            var waitingList = _context.WaitingLists.Where(w => w.BookId == bookId).OrderBy(w => w.DateAdded).ToList();
            var nextAvailabilityDate = waitingList.Any()
                ? waitingList.First().DateAdded.AddDays(30) // Assuming 30-day borrow period
                : (book.CopiesAvailable > 0 ? DateTime.Now : (DateTime?)null);

            var viewModel = new BorrowViewModel
            {
                Book = book,
                WaitingListCount = waitingList.Count,
                NextAvailabilityDate = nextAvailabilityDate
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowBookRequestAsync(int bookId)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
            {
                TempData["Error"] = "You must be logged in to borrow a book.";
                return RedirectToAction("Index");
            }

            var book = _context.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("Index");
            }

            if (book.CopiesAvailable > 0)
            {
                // Borrow the book directly
                var borrowedBook = new BorrowedBook
                {
                    BookId = bookId,
                    UserId = userId,
                    BorrowDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(30) // Default borrowing period: 30 days
                };

                book.CopiesAvailable--;
                _context.BorrowedBooks.Add(borrowedBook);
                _context.SaveChanges();

                // Send email notification
                await SendEmailAsync("ranayoun11@gmail.com", "Book Borrow Confirmation", $"You have successfully borrowed the book: {book.Title}. Please return it by {borrowedBook.ReturnDate.ToShortDateString()}.");

                TempData["Message"] = "You successfully borrowed the book!";
            }
            else
            {
                // Add the user to the waiting list
                return RedirectToAction("AddToWaitingList", new { bookId });
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnBook(int bookId)
        {
            int userId = GetCurrentUserId();

            var borrowedBook = _context.BorrowedBooks.FirstOrDefault(b => b.BookId == bookId && b.UserId == userId);
            if (borrowedBook != null)
            {
                _context.BorrowedBooks.Remove(borrowedBook);

                var book = _context.Books.SingleOrDefault(b => b.Id == bookId);
                if (book != null)
                {
                    book.CopiesAvailable++;

                    // Notify the first user in the waiting list
                    NotifyNextUserInWaitingList(bookId);
                }

                _context.SaveChanges();

                TempData["Message"] = "Book returned successfully.";
            }
            else
            {
                TempData["Error"] = "You have not borrowed this book.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult WaitingListView(int bookId)
        {
            var book = _context.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("Index");
            }

            var waitingList = _context.WaitingLists
                .Where(w => w.BookId == bookId)
                .OrderBy(w => w.DateAdded)
                .ToList() // Execute the query first
                .Select(w =>
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == w.UserId);
                    return new WaitingListViewModel
                    {
                        Username = user != null ? user.Username : "Unknown",
                        DateAdded = w.DateAdded
                    };
                })
                .ToList();


            var viewModel = new WaitingListPageViewModel
            {
                BookTitle = book.Title,
                WaitingList = waitingList
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToWaitingList(int bookId)
        {
            int userId;
            if (Session["UserId"] == null || !int.TryParse(Session["UserId"].ToString(), out userId))
            {
                TempData["Error"] = "You must be logged in to join the waiting list.";
                return RedirectToAction("Index");
            }

            var book = _context.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("Index");
            }

            var existingEntry = _context.WaitingLists.FirstOrDefault(w => w.BookId == bookId && w.UserId == userId);
            if (existingEntry == null)
            {
                var waitingEntry = new WaitingList
                {
                    BookId = bookId,
                    UserId = userId,
                    DateAdded = DateTime.Now
                };

                _context.WaitingLists.Add(waitingEntry);
                _context.SaveChanges();

                TempData["Message"] = "You have been added to the waiting list.";
            }
            else
            {
                TempData["Error"] = "You are already on the waiting list for this book.";
            }

            return RedirectToAction("WaitingListView", new { bookId });
        }


        private async Task NotifyNextUserInWaitingList(int bookId)
        {
            var nextInLine = _context.WaitingLists.Where(w => w.BookId == bookId).OrderBy(w => w.DateAdded).FirstOrDefault();
            if (nextInLine != null)
            {
                var user = _context.Users.SingleOrDefault(u => u.Id == nextInLine.UserId);
                if (user != null)
                {
                    await SendEmailAsync(user.Email, "Book Available for Borrowing", "The book you were waiting for is now available. Please borrow it within 5 days.");

                    _context.WaitingLists.Remove(nextInLine);
                    _context.SaveChanges();
                }
            }
        }


        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using (var smtp = new SmtpClient("smtp.gmail.com", 587)) // Use port 587 for TLS
                {
                    smtp.Credentials = new NetworkCredential("ranayoun11@gmail.com", "ubkg rapo lpjf pyjl");
                    smtp.EnableSsl = true; // Enable SSL/TLS

                    var mail = new MailMessage
                    {
                        From = new MailAddress("ranayoun11@gmail.com", "eBook Library"), // Optional: Add a display name
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true // Set to true if the email body contains HTML
                    };

                    mail.To.Add(toEmail);

                    // Send the email
                    await smtp.SendMailAsync(mail);

                    // Log success message
                    TempData["Message"] = "Email sent successfully.";
                }
            }
            catch (SmtpException smtpEx)
            {
                // Log SMTP-specific issues
                TempData["Error"] = $"SMTP Error: {smtpEx.Message}";
            }
            catch (Exception ex)
            {
                // Log general issues
                TempData["Error"] = $"Failed to send email: {ex.Message}";
            }
        }

        private int GetCurrentUserId()
        {
            if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int userId))
            {
                return userId;
            }
            return 0;
        }
    }
}
