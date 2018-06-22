namespace MoneyMerchantLocator.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Merchants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Contact = c.String(nullable: false),
                        Location = c.String(nullable: false),
                        LocationLat = c.Double(nullable: false),
                        LocationLng = c.Double(nullable: false),
                        SupportedNetworks = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 256),
                        FirstName = c.String(maxLength: 256),
                        LastName = c.String(maxLength: 256),
                        PasswordHash = c.String(nullable: false),
                        PasswordSalt = c.String(nullable: false),
                        DateRegistered = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Username, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "Username" });
            DropTable("dbo.Users");
            DropTable("dbo.Merchants");
        }
    }
}
