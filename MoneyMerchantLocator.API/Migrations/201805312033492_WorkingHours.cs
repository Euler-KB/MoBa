namespace MoneyMerchantLocator.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkingHours : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Merchants", "WorkingHours", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Merchants", "WorkingHours");
        }
    }
}
