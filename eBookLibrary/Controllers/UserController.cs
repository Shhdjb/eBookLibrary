using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using eBookLibrary.Models;
using System.Diagnostics;

namespace eBookLibrary.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult PersonalPage()
        {
            // Retrieve the user's ID from the session
            if (Session["UserId"] == null)
            {
                // If the session is null, redirect to the login page
                return RedirectToAction("Login", "Login");
            }

            // Parse the user ID from the session
            int userId = Convert.ToInt32(Session["UserId"]);

            // Debug log to confirm user ID
            Debug.WriteLine($"Debug: Session[\"UserId\"] = {userId}");

            try
            {
                // Fetch the user's personal library from the database
                var personalLibrary = _context.UserBooks
                    .Include(ub => ub.Book) // Include the related Book entity
                    .Where(ub => ub.UserId == userId) // Filter by UserId
                    .ToList();

                // Process borrowed books with expired BorrowEndDate
                var expiredBooks = personalLibrary
                    .Where(ub => ub.IsBorrowed && ub.BorrowEndDate.HasValue && ub.BorrowEndDate.Value < DateTime.Now)
                    .ToList();

                foreach (var expiredBook in expiredBooks)
                {
                    // Increment CopiesAvailable for the book
                    var book = _context.Books.SingleOrDefault(b => b.Id == expiredBook.BookId);
                    if (book != null)
                    {
                        book.CopiesAvailable++;
                    }

                    // Remove the book from the user's personal library
                    _context.UserBooks.Remove(expiredBook);
                }

                // Save changes to the database
                _context.SaveChanges();

                // Prepare the personal library to pass to the view
                var personalLibraryViewModel = personalLibrary
                    .Where(ub => !expiredBooks.Contains(ub)) // Exclude expired books
                    .Select(ub => new PersonalLibraryViewModel
                    {
                        BookId = ub.Book.Id,
                        Title = ub.Book.Title,
                        Author = ub.Book.Author,
                        CoverImagePath = ub.Book.CoverImagePath,
                        IsBorrowed = ub.IsBorrowed,
                        BorrowEndDate = ub.BorrowEndDate,
                        IsOwned = ub.IsOwned
                    })
                    .ToList();

                // Return the view with the personal library data
                return View(personalLibraryViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Debug.WriteLine($"Exception in PersonalPage: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Show a user-friendly error message
                ViewBag.ErrorMessage = "An error occurred while retrieving your personal library.";
                return View(new List<PersonalLibraryViewModel>()); // Return an empty library
            }
        }


        // Action to delete a book from the user's library
        [HttpPost]
        public ActionResult DeleteBook(int bookId)
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "You must be logged in to delete a book.";
                return RedirectToAction("Login", "Login");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            Debug.WriteLine($"Debug: Attempting to delete BookId {bookId} for UserId {userId}");

            try
            {
                // Find the book in the user's library
                var userBook = _context.UserBooks
                    .SingleOrDefault(ub => ub.BookId == bookId && ub.UserId == userId);

                if (userBook == null)
                {
                    TempData["Error"] = "Book not found in your library.";
                    return RedirectToAction("PersonalPage");
                }

                _context.UserBooks.Remove(userBook); // Remove the book
                _context.SaveChanges();

                TempData["Message"] = "Book removed from your library.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in DeleteBook: {ex.Message}");
                TempData["Error"] = "An error occurred while deleting the book.";
            }

            return RedirectToAction("PersonalPage");
        }

        [HttpPost]
        public ActionResult RateBook(int bookId, int rating)
        {
            if (Session["UserId"] == null)
            {
                TempData["Error"] = "You must be logged in to rate a book.";
                return RedirectToAction("Login", "Login");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            Debug.WriteLine($"Debug: Attempting to rate BookId {bookId} with rating {rating} for UserId {userId}");

            if (rating < 1 || rating > 5)
            {
                TempData["Error"] = "Rating must be between 1 and 5.";
                return RedirectToAction("PersonalPage");
            }

            try
            {
                // Check if the book exists in the user's library
                var userBook = _context.UserBooks
                    .Include(ub => ub.Book)
                    .SingleOrDefault(ub => ub.BookId == bookId && ub.UserId == userId);

                if (userBook == null || (!userBook.IsOwned && !userBook.IsBorrowed))
                {
                    TempData["Error"] = "You can only rate books in your library.";
                    return RedirectToAction("PersonalPage");
                }

                // Check if feedback already exists
                var existingFeedback = _context.Feedbacks
                    .SingleOrDefault(f => f.BookId == bookId && f.UserId == userId);

                if (existingFeedback != null)
                {
                    // Update existing feedback
                    existingFeedback.Rating = rating;
                    existingFeedback.DateAdded = DateTime.Now;
                }
                else
                {
                    // Add new feedback
                    var feedback = new Feedback
                    {
                        BookId = bookId,
                        UserId = userId,
                        Rating = rating,
                        DateAdded = DateTime.Now
                    };
                    _context.Feedbacks.Add(feedback);
                }

                _context.SaveChanges();
                TempData["Message"] = "Your feedback has been submitted.";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in RateBook: {ex.Message}");
                TempData["Error"] = "An error occurred while submitting your feedback.";
            }

            return RedirectToAction("PersonalPage");
        }

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
