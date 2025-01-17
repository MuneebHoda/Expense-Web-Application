using System.Data.Entity;
using System.Linq;
using ExpenseWebApplication.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Serilog;

public class AppDbInitializer
{
    public void Seed(AppDbContext context)
    {
        if (!context.Roles.Any())
        {
            SeedRoles(context);
        }
        else
        {
            Log.Information("Roles already exist, skipping seeding.");
        }
        if (!context.Users.Any(u => u.Roles.Any(r => r.RoleId == "4")))
        {
            SeedManagerUsers(context);
            Log.Information("Manager users seeded successfully.");
        }
        else
        {
            Log.Information("Managers already exist, skipping seeding.");
        }
        if (!context.Users.Any(u => u.Roles.Any(r => r.RoleId == "3")))
        {
            SeedAdminUser(context);
            Log.Information("Admin users seeded successfully.");
        }
        else
        {
            Log.Information("Admin already exist, skipping seeding.");
        }
    }

    private void SeedRoles(AppDbContext context)
    {
        Log.Information("Seeding Roles...");

        var roles = new[]
        {
            new IdentityRole { Name = "Employee" },
            new IdentityRole { Name = "Accountant" },
            new IdentityRole { Name = "Admin" },
            new IdentityRole { Name = "Manager" }
        };

        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

        foreach (var role in roles)
        {
            if (!roleManager.RoleExists(role.Name))
            {
                var result = roleManager.Create(role);
                if (!result.Succeeded)
                {
                    Log.Error("Failed to create role {Role}: {Errors}", role.Name, string.Join(", ", result.Errors));
                }
                else
                {
                    Log.Information("Role {Role} created successfully.", role.Name);
                }
            }
            else
            {
                Log.Information("Role {Role} already exists, skipping creation.", role.Name);
            }
        }
    }

    private void SeedManagerUsers(AppDbContext context)
    {
        var managerRole = context.Roles.FirstOrDefault(r => r.Name == "Manager");

        if (managerRole == null || !context.Users.Any(u => u.Roles.Any(r => r.RoleId == managerRole.Id)))
        {
            Log.Information("Seeding Manager users...");

            var userManager = new UserManager<Users>(new UserStore<Users>(context));

            var managers = new[]
            {
                new Users
                {
                    UserName = "manager1@example.com",
                    Email = "manager1@example.com",
                    UserCode = 1001,
                    RoleId = "4",
                    Age = 45,
                    Gender = "Male",
                    Department = "VeriTouch",
                    ManagerId = null
                },
                new Users
                {
                    UserName = "manager2@example.com",
                    Email = "manager2@example.com",
                    UserCode = 1002,
                    RoleId="4",
                    Age = 38,
                    Gender = "Female",
                    Department = "VeriLink",
                    ManagerId = null
                }
            };

            foreach (var manager in managers)
            {
                if (userManager.FindByName(manager.UserName) == null)
                {
                    var result = userManager.Create(manager, "Password@123");
                    if (result.Succeeded)
                    {
                        userManager.AddToRole(manager.Id, "Manager"); 
                        Log.Information("Manager {Email} created successfully.", manager.Email);
                    }
                    else
                    {
                        Log.Error("Failed to create manager {Email}: {Errors}", manager.Email, string.Join(", ", result.Errors));
                    }
                }
                else
                {
                    Log.Information("Manager {Email} already exists, skipping creation.", manager.Email);
                }
            }
        }
        else
        {
            Log.Information("Managers already exist, skipping seeding.");
        }
    }

    private void SeedAdminUser(AppDbContext context)
    {
        var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");

        if (adminRole == null || !context.Users.Any(u => u.Roles.Any(r => r.RoleId == adminRole.Id)))
        {
            Log.Information("Seeding Admin user...");

            var userManager = new UserManager<Users>(new UserStore<Users>(context));

            var admin = new Users
            {
                UserName = "Admin",
                Email = "admin@example.com",
                UserCode = 1,
                RoleId = "3",
                Age = 40,
                Gender = "Male",
                Department = "Administration",
                ManagerId = null
            };

            if (userManager.FindByName(admin.UserName) == null)
            {
                var result = userManager.Create(admin, "Admin@123");
                if (result.Succeeded)
                {
                    userManager.AddToRole(admin.Id, "Admin");
                    Log.Information("Admin {Email} created successfully.", admin.Email);
                }
                else
                {
                    Log.Error("Failed to create admin {Email}: {Errors}", admin.Email, string.Join(", ", result.Errors));
                }
            }
            else
            {
                Log.Information("Admin {Email} already exists, skipping creation.", admin.Email);
            }
        }
        else
        {
            Log.Information("Admin user already exists, skipping seeding.");
        }
    }
}
