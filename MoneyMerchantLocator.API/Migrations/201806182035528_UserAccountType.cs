namespace MoneyMerchantLocator.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UserAccountType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AccountType", c => c.Int(nullable: false, defaultValue: (int)Models.AccountType.Administrator));
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "AccountType");
        }
    }
}
