namespace eBookLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class SyncSchemaFixed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Admins", "Role", c => c.String(defaultValue: "Admin", maxLength: 50));
            AddColumn("dbo.Admins", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "Role", c => c.String());
            AddColumn("dbo.Users", "CreatedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Users", "Username", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.Users", "PasswordHash", c => c.String(nullable: false, maxLength: 64));
            CreateIndex("dbo.Users", "Email", unique: true);
        }

        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "Email" });
            AlterColumn("dbo.Users", "PasswordHash", c => c.String());
            AlterColumn("dbo.Users", "Email", c => c.String());
            AlterColumn("dbo.Users", "Username", c => c.String());
            DropColumn("dbo.Users", "CreatedAt");
            DropColumn("dbo.Users", "Role");
            DropColumn("dbo.Admins", "CreatedAt");
            DropColumn("dbo.Admins", "Role");
        }
    }
}
