using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using ExpenseWebApplication.Models;
public class AppDbContext : IdentityDbContext<Users>
{
    public AppDbContext() : base("ExpenseAppDbContext") 
    { 
        
    }

    public DbSet<Roles> UserRoles {  get; set; }
    public DbSet<Tokens> Tokens { get; set; }
    public DbSet<Expenses>  Expenses { get; set; }
    public DbSet<ExpenseForms> ExpenseForms { get; set; }
    public DbSet<ApprovalProcessTable> ApprovalProcessTable {  get; set; }
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Users>()
            .HasOptional(u => u.Manager)
            .WithMany()
            .HasForeignKey(u => u.ManagerId);

        modelBuilder.Entity<Users>()
           .HasOptional(u => u.Role)
           .WithMany()
           .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<ExpenseForms>()
            .HasMany(f => f.Expenses)
            .WithRequired(e => e.ExpenseForm)
            .HasForeignKey(e => e.ExpenseFormID);
    }
}