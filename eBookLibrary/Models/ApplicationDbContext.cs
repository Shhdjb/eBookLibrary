using System.Data.Entity;

namespace eBookLibrary.Models
{
    public class ApplicationDbContext : DbContext
    {

        // Constructor connects to the DefaultConnection defined in Web.config
        public ApplicationDbContext() : base("DefaultConnection")
        {
            // Optionally configure the database initializer if needed
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        // DbSet property for the Users table
        public DbSet<User> Users { get; set; }

        // Add DbSet for UserBooks and Feedbacks
        public DbSet<UserBook> UserBooks { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }


        // DbSet for Books
        public DbSet<Book> Books { get; set; }
        public DbSet<WaitingList> WaitingLists { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }
        public DbSet<Purchase> Purchases { get; set; }


        // DbSet for Admins
        public DbSet<Admin> Admins { get; set; }

        // Optionally override OnModelCreating if needed for advanced configurations
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Ensure base configuration is called
            base.OnModelCreating(modelBuilder);

            // Example configuration for the User entity
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(64);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Repeat similar configurations for other entities as needed
        }
    }
}

