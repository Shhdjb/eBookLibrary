namespace eBookLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserIdType : DbMigration
    {
        public override void Up()
        {
            // CreateTable(
            //  "dbo.Feedbacks",
            //   c => new
            //  {
            //  Id = c.Int(nullable: false, identity: true),
            //  UserId = c.String(),
            //  BookId = c.Int(nullable: false),
            //  Rating = c.Int(nullable: false),
            //  DateAdded = c.DateTime(nullable: false),
            //   })
            //   .PrimaryKey(t => t.Id);

            // CreateTable(
            //  "dbo.UserBooks",
            //  c => new
            //          {
            //  Id = c.Int(nullable: false, identity: true),
            //  UserId = c.Int(nullable: false),
            //  BookId = c.Int(nullable: false),
            //  IsBorrowed = c.Boolean(nullable: false),
            //  BorrowEndDate = c.DateTime(),
            //  IsOwned = c.Boolean(nullable: false),
            //  })
            //     .PrimaryKey(t => t.Id)
            //     .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
            //       .Index(t => t.BookId);

            //  AddColumn("dbo.Books", "DiscountStartDate", c => c.DateTime());
            //   AddColumn("dbo.Books", "DiscountEndDate", c => c.DateTime());
            //  DropColumn("dbo.Books", "InStock");
        }

        public override void Down()
        {
            AddColumn("dbo.Books", "InStock", c => c.Int(nullable: false));
            DropForeignKey("dbo.UserBooks", "BookId", "dbo.Books");
            DropIndex("dbo.UserBooks", new[] { "BookId" });
            DropColumn("dbo.Books", "DiscountEndDate");
            DropColumn("dbo.Books", "DiscountStartDate");
            // DropTable("dbo.UserBooks");
            // DropTable("dbo.Feedbacks");
        }
    }
}
