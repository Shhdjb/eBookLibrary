using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using eBookLibrary.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace eBookLibrary.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController()
        {
            _context = new ApplicationDbContext();
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User model)
        {
            try
            {
                // Validate input data
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.PasswordHash))
                {
                    ViewBag.Message = "All fields are required.";
                    return View(model);
                }

                if (model.Username.Length < 3 || model.Username.Length > 50)
                {
                    ViewBag.Message = "Username must be between 3 and 50 characters.";
                    return View(model);
                }

                if (!new EmailAddressAttribute().IsValid(model.Email))
                {
                    ViewBag.Message = "Invalid email format.";
                    return View(model);
                }

                if (model.PasswordHash.Length < 6)
                {
                    ViewBag.Message = "Password must be at least 6 characters long.";
                    return View(model);
                }

                // Check if email is already registered
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ViewBag.Message = "Email is already registered.";
                    return View(model);
                }

                // Hash the password
                var hashedPassword = HashPassword(model.PasswordHash);

                // Create a new user
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Role = "User", // Default role
                    CreatedAt = DateTime.Now
                };

                // Save the user to the database
                _context.Users.Add(user);
                _context.SaveChanges();

                // Redirect to the Login page
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                // Log the exception (optional for debugging purposes)
                System.Diagnostics.Debug.WriteLine($"Exception in SignUp: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Show a friendly error message
                ViewBag.Message = "An error occurred while processing your request. Please try again.";
                return View(model);
            }
        }


        // Helper method to hash the password securely
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // Dispose the database context to release resources
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
