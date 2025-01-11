using System.Linq;
using System.Web.Mvc;
using eBookLibrary.Models;
using System;
using System.Web;
using System.IO;
using System.Collections.Generic;

namespace eBookLibrary.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController()
        {
            _context = new ApplicationDbContext();
        }

        // Admin Dashboard
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // GET: AddBook
        [HttpGet]
        public ActionResult AddBook()
        {
            // Populate formats for the dropdown list
            ViewBag.Formats = new List<string> { "EPUB", "FB2", "MOBI", "PDF" };
            return View();
        }

        [HttpPost]
        public ActionResult AddBook(Book book, HttpPostedFileBase CoverImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Handle image upload
                    if (CoverImage != null && CoverImage.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(CoverImage.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/Content/Images/Covers"), fileName);
                        Directory.CreateDirectory(Server.MapPath("~/Content/Images/Covers"));
                        CoverImage.SaveAs(filePath);
                        book.CoverImagePath = fileName; // Save the file path in the database
                    }

                    // Save the book to the database
                    _context.Books.Add(book);
                    _context.SaveChanges();

                    TempData["Message"] = "Book added successfully!";
                    return RedirectToAction("ManageCatalog");
                }

                TempData["Error"] = "Please fill in all required fields.";
                ViewBag.Formats = new List<string> { "EPUB", "FB2", "MOBI", "PDF" };
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while adding the book: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        // GET: ManageCatalog
        [HttpGet]
        public ActionResult ManageCatalog()
        {
            var books = _context.Books.ToList();
            return View(books);
        }

        // DELETE: Delete Book
        [HttpPost]
        public ActionResult DeleteBook(int id)
        {
            try
            {
                var book = _context.Books.SingleOrDefault(b => b.Id == id);
                if (book == null)
                {
                    TempData["Error"] = "Book not found.";
                    return RedirectToAction("ManageCatalog");
                }

                _context.Books.Remove(book);
                _context.SaveChanges();

                TempData["Message"] = "Book deleted successfully!";
                return RedirectToAction("ManageCatalog");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while deleting the book: {ex.Message}";
                return RedirectToAction("ManageCatalog");
            }
        }

        [HttpGet]
        public ActionResult EditBook(int id)
        {
            // Retrieve the book by ID
            var book = _context.Books.SingleOrDefault(b => b.Id == id);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction("ManageCatalog");
            }

            // Populate formats for the dropdown list
            ViewBag.Formats = new List<string> { "EPUB", "FB2", "MOBI", "PDF" };

            return View(book); // Return the EditBook view with the book model
        }

        // POST: EditBook
        [HttpPost]
        public ActionResult EditBook(Book book)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please fill in all required fields.";
                    ViewBag.Formats = new List<string> { "EPUB", "FB2", "MOBI", "PDF" }; // Re-populate formats in case of error
                    return View(book); // Return the edit view with validation errors
                }

                // Fetch the book from the database
                var bookInDb = _context.Books.SingleOrDefault(b => b.Id == book.Id);
                if (bookInDb == null)
                {
                    TempData["Error"] = "Book not found.";
                    return RedirectToAction("ManageCatalog");
                }

                // Update the fields with the new values
                bookInDb.Title = book.Title;
                bookInDb.Author = book.Author;
                bookInDb.Genre = book.Genre;
                bookInDb.BuyPrice = book.BuyPrice;
                bookInDb.BorrowPrice = book.BorrowPrice;
                bookInDb.DiscountPrice = book.DiscountPrice;
                bookInDb.DiscountStartDate = book.DiscountStartDate;
                bookInDb.DiscountEndDate = book.DiscountEndDate;

                // Save changes to the database
                _context.SaveChanges();

                TempData["Message"] = "Book updated successfully!";
                return RedirectToAction("ManageCatalog"); // Redirect to the catalog after success
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while updating the book. Please try again. Error: {ex.Message}";
                ViewBag.Formats = new List<string> { "EPUB", "FB2", "MOBI", "PDF" }; // Re-populate formats in case of error
                return View(book); // Return to the edit view in case of an exception
            }
        }


        // Notifications for Waiting List
        public void NotifyUser(int bookId)
        {
            try
            {
                var waitingList = _context.WaitingLists.Where(w => w.BookId == bookId).OrderBy(w => w.DateAdded).ToList();
                if (waitingList.Any())
                {
                    var nextUser = waitingList.First();
                    // Logic to notify user (e.g., email or system notification)
                    TempData["Message"] = $"User {nextUser.UserId} has been notified about the availability of the book.";
                    _context.WaitingLists.Remove(nextUser);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while notifying the user: {ex.Message}";
            }
        }

        // Reminder for Borrowed Books
        public void SendReminder()
        {
            try
            {
                var borrowedBooks = _context.BorrowedBooks.Where(b => b.ReturnDate <= DateTime.Now.AddDays(5)).ToList();
                foreach (var borrowedBook in borrowedBooks)
                {
                    // Logic to send reminder (e.g., email or system notification)
                    TempData["Message"] = $"Reminder sent to User {borrowedBook.UserId} for book {borrowedBook.BookId}.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while sending reminders: {ex.Message}";
            }
        }

        // Dispose resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
