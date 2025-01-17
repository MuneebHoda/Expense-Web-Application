using Microsoft.AspNet.Identity;
using ExpenseWebApplication.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System;
using Serilog;
using System.Linq;

namespace ExpenseWebApplication.Services
{
    public class UserServices
    {
        private readonly UserManager<Users> _userManager;

        public UserServices()
        {
            var store = new UserStore<Users>(new AppDbContext());
            _userManager = new UserManager<Users>(store);
        }

        public async Task<IdentityResult> Register(string username, string email, string password, string age, string gender, string department, string roleName)
        {
            try
            {
                string roleId = GetRoleIdByName(roleName);

                Log.Information("SignUp process started for {Email}", email);
                var newUser = new Users
                {
                    UserName = username,
                    Email = email,
                    Age = int.Parse(age),
                    Gender = gender,
                    Department = department,
                    RoleId = roleId,
                    ManagerId = null
                };
                var result = await _userManager.CreateAsync(newUser, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser.Id, roleName);
                    Log.Information("SignUp process completed successfully for {Email}", email);
                }else{
                    Log.Error("SignUp process failed for {Email} with errors: {Errors}", email);
                    foreach (var error in result.Errors) 
                    {
                        Log.Error("SignUp Error: {Error}", error);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SignUp process encountered an exception for {Email}", email);
                return IdentityResult.Failed();
            }
        }

        public async Task<Users> Login(string email, string password)
        {
            try
            {
                Log.Information("Login process started for {Email}", email);
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
                    if (isPasswordValid)
                    {
                        Log.Information("Login successful for {Email}", email);

                        return user;
                    }
                    Log.Warning("Invalid password attempt for {Email}", email);
                }
                else 
                {
                    Log.Warning("User not found for {Email}", email);
                }

                return null;
            }
            catch (Exception ex) {
                Log.Error(ex, "Login process encountered an exception for {Email}", email);
                return null;
            }
        }

        private string GetRoleIdByName(string roleName)
        {
            if (roleName == "Employee")
            {
                return "1";
            }
            else             {
                return "2";
            }
        }
    }
}
