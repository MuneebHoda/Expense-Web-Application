namespace ExpenseWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpensesExpensesFormsApprovalProcessTableCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApprovalProcessTables",
                c => new
                    {
                        ApprovalID = c.Int(nullable: false, identity: true),
                        ManagerApprovalStatus = c.String(nullable: false),
                        ManagerComments = c.String(),
                        ManagerApprovalDate = c.DateTime(),
                        AccountantApprovalStatus = c.String(nullable: false),
                        AccountantApprovalDate = c.DateTime(),
                        IsPaid = c.Boolean(nullable: false),
                        ManagerID = c.String(maxLength: 128),
                        AccountantID = c.String(maxLength: 128),
                        ExpenseForm_FormID = c.Int(),
                    })
                .PrimaryKey(t => t.ApprovalID)
                .ForeignKey("dbo.AspNetUsers", t => t.AccountantID)
                .ForeignKey("dbo.ExpenseForms", t => t.ExpenseForm_FormID)
                .ForeignKey("dbo.AspNetUsers", t => t.ManagerID)
                .Index(t => t.ManagerID)
                .Index(t => t.AccountantID)
                .Index(t => t.ExpenseForm_FormID);
            
            CreateTable(
                "dbo.ExpenseForms",
                c => new
                    {
                        FormID = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManagerComments = c.String(),
                        SubmittedDate = c.DateTime(nullable: false),
                        ApprovalDate = c.DateTime(),
                        PaymentTime = c.DateTime(),
                        EmployeeID = c.String(maxLength: 128),
                        ManagerID = c.String(maxLength: 128),
                        AccountantID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.FormID)
                .ForeignKey("dbo.AspNetUsers", t => t.AccountantID)
                .ForeignKey("dbo.AspNetUsers", t => t.EmployeeID)
                .ForeignKey("dbo.AspNetUsers", t => t.ManagerID)
                .Index(t => t.EmployeeID)
                .Index(t => t.ManagerID)
                .Index(t => t.AccountantID);
            
            CreateTable(
                "dbo.Expenses",
                c => new
                    {
                        ExpenseID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Currency = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        UserId = c.String(maxLength: 128),
                        ExpenseFormID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ExpenseID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.ExpenseForms", t => t.ExpenseFormID, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ExpenseFormID);
            
            AddColumn("dbo.AspNetUsers", "UserCode", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserID", c => c.Int(nullable: false));
            DropForeignKey("dbo.ApprovalProcessTables", "ManagerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApprovalProcessTables", "ExpenseForm_FormID", "dbo.ExpenseForms");
            DropForeignKey("dbo.ExpenseForms", "ManagerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Expenses", "ExpenseFormID", "dbo.ExpenseForms");
            DropForeignKey("dbo.Expenses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExpenseForms", "EmployeeID", "dbo.AspNetUsers");
            DropForeignKey("dbo.ExpenseForms", "AccountantID", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApprovalProcessTables", "AccountantID", "dbo.AspNetUsers");
            DropIndex("dbo.Expenses", new[] { "ExpenseFormID" });
            DropIndex("dbo.Expenses", new[] { "UserId" });
            DropIndex("dbo.ExpenseForms", new[] { "AccountantID" });
            DropIndex("dbo.ExpenseForms", new[] { "ManagerID" });
            DropIndex("dbo.ExpenseForms", new[] { "EmployeeID" });
            DropIndex("dbo.ApprovalProcessTables", new[] { "ExpenseForm_FormID" });
            DropIndex("dbo.ApprovalProcessTables", new[] { "AccountantID" });
            DropIndex("dbo.ApprovalProcessTables", new[] { "ManagerID" });
            DropColumn("dbo.AspNetUsers", "UserCode");
            DropTable("dbo.Expenses");
            DropTable("dbo.ExpenseForms");
            DropTable("dbo.ApprovalProcessTables");
        }
    }
}
