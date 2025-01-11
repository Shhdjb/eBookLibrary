using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using eBookLibrary.Models;
using System.Diagnostics; // For Debugging

namespace eBookLibrary.Controllers
{
    public class PersonalLibraryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonalLibraryController()
        {
            _context = new ApplicationDbContext();
        }

        // Action to display the user's personal library
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "User is not logged in.";
                return RedirectToAction("Login", "Account");
            }

            // Fetch the UserId
            int userId = Convert.ToInt32(Session["UserId"]);

            // Fetch user's personal library with associated book data
            var personalLibrary = _context.UserBooks
                .Include(ub => ub.Book) // Include Book navigation property
                .Where(ub => ub.UserId == userId)
                .Select(ub => new PersonalLibraryViewModel
                {
                    BookId = ub.Book.Id,
                    Title = ub.Book.Title,
                    Author = ub.Book.Author,
                    CoverImagePath = ub.Book.CoverImagePath,
                    IsBorrowed = ub.IsBorrowed,
                    BorrowEndDate = ub.BorrowEndDate,
                    IsOwned = ub.IsOwned,
                    Format = ub.Book.Formats // Fetch the format field
        })
                .ToList();

            if (!personalLibrary.Any())
            {
                TempData["Error"] = "Your personal library is empty.";
            }

            return View(personalLibrary);
        }


        // Action to delete a book from the personal library
        [HttpPost]
        public ActionResult DeleteBook(int bookId)
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "User is not logged in.";
                return RedirectToAction("Login", "Account");
            }

            // Fetch the UserId from the session
            int userId = Convert.ToInt32(Session["UserId"]);
            Debug.WriteLine($"Debug: Attempting to delete BookId {bookId} for UserId {userId}");

            // Find the user's book record based on the BookId and UserId
            var userBook = _context.UserBooks.SingleOrDefault(ub =>
                ub.BookId == bookId &&
                ub.UserId == userId); // Filter by integer UserId

            if (userBook != null)
            {
                // Remove the book from the user's library
                _context.UserBooks.Remove(userBook);
                _context.SaveChanges();
                TempData["Message"] = "Book removed from your library.";
            }
            else
            {
                TempData["Error"] = "Book not found.";
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult DownloadBook(int bookId)
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "User is not logged in.";
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var userBook = _context.UserBooks
                .Include(ub => ub.Book)
                .SingleOrDefault(ub => ub.BookId == bookId && ub.UserId == userId);

            if (userBook == null)
            {
                TempData["Error"] = "Book not found in your library.";
                return RedirectToAction("Index");
            }

            var book = userBook.Book;

            if (string.IsNullOrEmpty(book.Formats))
            {
                TempData["Error"] = "This book does not have a downloadable format.";
                return RedirectToAction("Index");
            }

            // Path to the book file
            var filePath = Server.MapPath($"~/Content/Books/{book.Title}.{book.Formats.ToLower()}");

            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "The book file does not exist.";
                return RedirectToAction("Index");
            }

            // Return the file for download
            return File(filePath, "application/octet-stream", $"{book.Title}.{book.Formats.ToLower()}");
        }



        // Action to rate a book in the personal library
        [HttpPost]
        public ActionResult RateBook(int bookId, int rating)
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "User is not logged in.";
                return RedirectToAction("Login", "Account");
            }

            // Fetch the UserId from the session
            int userId = Convert.ToInt32(Session["UserId"]);

            Debug.WriteLine($"Debug: Attempting to rate BookId {bookId} with rating {rating} for UserId {userId}");

            // Ensure the user owns or borrowed the book
            var userBook = _context.UserBooks
                .Include(ub => ub.Book)
                .SingleOrDefault(ub => ub.BookId == bookId && ub.UserId == userId);

            if (userBook != null && (userBook.IsOwned || userBook.IsBorrowed))
            {
                // Add feedback entry
                var feedback = new Feedback
                {
                    BookId = bookId,
                    UserId = userId,
                    Rating = rating,
                    DateAdded = DateTime.Now
                };

                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();
                TempData["Message"] = "Your feedback has been submitted.";
                Debug.WriteLine($"Debug: Feedback submitted for BookId {bookId} with rating {rating}");
            }
            else
            {
                TempData["Error"] = "You can only rate books in your library.";
                Debug.WriteLine($"Debug: UserId {userId} attempted to rate BookId {bookId} not in their library.");
            }

            return RedirectToAction("Index");
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
