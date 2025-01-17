namespace ExpenseWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TokenTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Token = c.String(),
                        JwtId = c.String(),
                        IsRevoked = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                        DateExpire = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tokens", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Tokens", new[] { "UserId" });
            DropTable("dbo.Tokens");
        }
    }
}
