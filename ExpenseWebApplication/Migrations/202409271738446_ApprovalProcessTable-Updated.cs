namespace ExpenseWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApprovalProcessTableUpdated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApprovalProcessTables", "ExpenseForm_FormID", "dbo.ExpenseForms");
            DropIndex("dbo.ApprovalProcessTables", new[] { "ExpenseForm_FormID" });
            RenameColumn(table: "dbo.ApprovalProcessTables", name: "ExpenseForm_FormID", newName: "ExpenseFormID");
            AlterColumn("dbo.ApprovalProcessTables", "ExpenseFormID", c => c.Int(nullable: false));
            CreateIndex("dbo.ApprovalProcessTables", "ExpenseFormID");
            AddForeignKey("dbo.ApprovalProcessTables", "ExpenseFormID", "dbo.ExpenseForms", "FormID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApprovalProcessTables", "ExpenseFormID", "dbo.ExpenseForms");
            DropIndex("dbo.ApprovalProcessTables", new[] { "ExpenseFormID" });
            AlterColumn("dbo.ApprovalProcessTables", "ExpenseFormID", c => c.Int());
            RenameColumn(table: "dbo.ApprovalProcessTables", name: "ExpenseFormID", newName: "ExpenseForm_FormID");
            CreateIndex("dbo.ApprovalProcessTables", "ExpenseForm_FormID");
            AddForeignKey("dbo.ApprovalProcessTables", "ExpenseForm_FormID", "dbo.ExpenseForms", "FormID");
        }
    }
}
