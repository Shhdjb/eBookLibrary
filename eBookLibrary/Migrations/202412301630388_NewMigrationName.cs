namespace eBookLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.SqlClient;

    public partial class NewMigrationName : DbMigration
    {
        public override void Up()
        {
            // Check if the 'Admins' table exists before attempting to create it
            if (!TableExists("Admins"))
            {
                CreateTable(
                    "dbo.Admins",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true), // Ensure 'Id' is the only identity column
                        Username = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100),
                        PasswordHash = c.String(nullable: false),
                    })
                    .PrimaryKey(t => t.Id); // Define the primary key as 'Id'
            }
            else
            {
                // If the table exists, ensure the schema is correct
                AlterColumn("dbo.Admins", "Username", c => c.String(nullable: false, maxLength: 100));
                AlterColumn("dbo.Admins", "Email", c => c.String(nullable: false, maxLength: 100));
                AlterColumn("dbo.Admins", "PasswordHash", c => c.String(nullable: false));
            }

            // Add 'DiscountPrice' column to the 'Books' table if it doesn't already exist
            if (!ColumnExists("dbo.Books", "DiscountPrice"))
            {
                AddColumn("dbo.Books", "DiscountPrice", c => c.Decimal(precision: 18, scale: 2));
            }
        }

        // Helper methods to check for table and column existence
        private bool TableExists(string tableName)
        {
            using (var connection = new SqlConnection("Server=DESKTOP-B2G8NTC\\SQLEXPRESS;Database=eBookLibrary;Trusted_Connection=True;"))
            {
                connection.Open();
                var query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
                using (var command = new SqlCommand(query, connection))
                {
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        private bool ColumnExists(string tableName, string columnName)
        {
            using (var connection = new SqlConnection("Server=DESKTOP-B2G8NTC\\SQLEXPRESS;Database=eBookLibrary;Trusted_Connection=True;"))
            {
                connection.Open();
                var query = $@"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";
                using (var command = new SqlCommand(query, connection))
                {
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public override void Down()
        {
            DropColumn("dbo.Books", "DiscountPrice");

            // Only drop if the constraint is confirmed to exist
            // Uncomment the below line if necessary
            // Sql("ALTER TABLE Admins DROP CONSTRAINT PK_dbo_Admins");

            DropTable("dbo.Admins");
        }
    }
}