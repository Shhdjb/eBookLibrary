namespace eBookLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using eBookLibrary.Models;

    public partial class AddSearchAndSortingFields : DbMigration
    {
        public override void Up()
        {
            // Add new columns to the existing Books table
            AddColumn("dbo.Books", "Genre", c => c.String());
            AddColumn("dbo.Books", "YearOfPublishing", c => c.Int(nullable: false));
            AddColumn("dbo.Books", "AgeLimit", c => c.Int(nullable: false));
            AddColumn("dbo.Books", "Popularity", c => c.Int(nullable: false));
            AddColumn("dbo.Books", "CoverImageUrl", c => c.String());

            // Check if the Users table exists before attempting to create it
            if (!TableExists("Users"))
            {
                CreateTable(
                    "dbo.Users",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Email = c.String(),
                        PasswordHash = c.String(),
                    })
                    .PrimaryKey(t => t.Id);
            }
        }

        public override void Down()
        {
            // Remove the new columns from the Books table
            DropColumn("dbo.Books", "CoverImageUrl");
            DropColumn("dbo.Books", "Popularity");
            DropColumn("dbo.Books", "AgeLimit");
            DropColumn("dbo.Books", "YearOfPublishing");
            DropColumn("dbo.Books", "Genre");

            // Drop the Users table only if it was created by this migration
            if (TableExists("Users"))
            {
                DropTable("dbo.Users");
            }
        }

        // Helper method to check if a table exists
        private bool TableExists(string tableName)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
                var command = context.Database.Connection.CreateCommand();
                command.CommandText = query;

                context.Database.Connection.Open();
                var result = (int)command.ExecuteScalar();
                context.Database.Connection.Close();

                return result > 0;
            }
        }
    }
}
