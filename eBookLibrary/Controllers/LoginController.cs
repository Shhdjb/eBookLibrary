using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using eBookLibrary.Models;
using System;

namespace eBookLibrary.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Message = "Email and password are required.";
                    return View();
                }

                // Hash the entered password
                var hashedPassword = HashPassword(password);

                // Check if the user is an admin
                var adminAccount = _context.Admins.SingleOrDefault(a => a.Email == email && a.PasswordHash == hashedPassword);
                if (adminAccount != null)
                {
                    if (string.IsNullOrEmpty(adminAccount.Role) || adminAccount.Role != "Admin")
                    {
                        ViewBag.Message = "Invalid role configuration for this account.";
                        return View();
                    }

                    // Store admin information in session
                    Session["UserId"] = adminAccount.Id;
                    Session["Username"] = adminAccount.Username;
                    Session["Role"] = adminAccount.Role;

                    // Redirect to the Admin page
                    return RedirectToAction("Index", "Admin");
                }


                // Check if the user is a regular user
                var userAccount = _context.Users.SingleOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);
                if (userAccount != null)
                {
                    // Store user information in session
                    Session["UserId"] = userAccount.Id;
                    Session["Username"] = userAccount.Username;
                    Session["Role"] = "User";

                    // Redirect to the All Books page
                    return RedirectToAction("Index", "Books");
                }

                // If authentication fails, show an error message
                ViewBag.Message = "Invalid email or password.";
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Exception in Login: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Show a user-friendly error message
                ViewBag.Message = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }

        // Log Out action to clear the session and redirect to the login page
        public ActionResult LogOut()
        {
            // Clear session data
            Session.Clear();

            // Redirect to the Login page
            return RedirectToAction("Login", "Login");
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

