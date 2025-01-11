namespace eBookLibrary.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using eBookLibrary.Models;
    using System.Security.Cryptography;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<eBookLibrary.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            // Seed an admin user
            if (!context.Admins.Any())
            {
                context.Admins.Add(new Admin
                {
                    Username = "NewAdminName",
                    Email = "newemail@ebooklibrary.com",
                    PasswordHash = HashPassword("NewSecurePassword123"),
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                });

            }

            base.Seed(context);
        }

        // Helper method to hash passwords
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}