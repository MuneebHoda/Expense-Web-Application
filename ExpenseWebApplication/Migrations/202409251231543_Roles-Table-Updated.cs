namespace ExpenseWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RolesTableUpdated : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetRoles", "RoleName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetRoles", "RoleName", c => c.String());
        }
    }
}
