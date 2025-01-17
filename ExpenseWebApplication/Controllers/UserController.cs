using ExpenseWebApplication.Services;
using ExpenseWebApplication.Models;
using Serilog;
using System;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ExpenseWebApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly UserServices _userServices;
        private readonly TokenService _tokenServices;
        public UserController() 
        {
            _userServices = new UserServices();
            _tokenServices = new TokenService();
        }

        [AllowAnonymous]
        // GET: User
        public ActionResult Login()
        {
            ViewBag.ShowNavbar = false;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.ShowNavbar = false;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.ShowNavbar = false;
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string username, string email, string password, string confirmPassword, string age, string gender, string department, string position)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model state for registration attempt by {Email}", email);
                    TempData["SignUpErrorMessage"] = "Invalid data entered.";
                    return RedirectToAction("Register");
                }

                if (password != confirmPassword)
                {
                    Log.Warning("Password mismatch during registration for {Email}", email);
                    TempData["SignUpErrorMessage"] = "Passwords do not match.";
                    return RedirectToAction("Register");
                }

                Log.Information("Registration attempt started for {Email}", email);
                var result = await _userServices.Register(username, email, password, age, gender, department, position);

                if (result.Succeeded)
                {
                    Log.Information("User {Email} registered successfully", email);
                    TempData["SuccessSignUpMessage"] = "User created successfully.";
                    return RedirectToAction("Login");
                }
                else
                {
                    Log.Warning("User creation failed for {Email}", email);
                    TempData["SignUpErrorMessage"] = "User creation failed.";
                    return RedirectToAction("Register");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during registration for {Email}", email);
                TempData["SignUpErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToAction("Register");
            }
        }

        //POST: User/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string password) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model state for login attempt by {Email}", email);
                    TempData["LoginErrorMessage"] = "Invalid data entered.";
                    return RedirectToAction("Login");
                }

                Log.Information("Login attempt started for {Email}", email);
                var user = await _userServices.Login(email, password);

                if (user != null)
                {
                    Log.Information("User {Email} logged in successfully", email);

                    var accessToken = _tokenServices.GenerateAccessToken(user);
                    var refreshToken = _tokenServices.GenerateRefreshToken(user);

                    SetTokenCookie("AccessToken", accessToken.Token, accessToken.DateExpire);
                    SetTokenCookie("RefreshToken", refreshToken.Token, refreshToken.DateExpire);

                    TempData["LoginSuccessMessage"] = "User logged in successfully.";
                    Log.Information("Directing user based on role.");
                    return RedirectToActionBasedOnResult(user.RoleId);
                }
                else 
                { 
                    Log.Warning("Login failed for {Email}.Incorrect email or password.",email);
                    TempData["LoginErrorMessage"] = "Invalid email or password.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex) 
            {
                Log.Error(ex, "An error occured during the login process for {Email}", email);
                TempData["LoginErrorMessage"] = "An error occured. Please try again.";
                return RedirectToAction("Login");
            }
        }

        //POST: User/Logout
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            if (User.Identity.IsAuthenticated) 
            {
                if (Request.Cookies["AccessToken"] != null)
                {
                    Response.Cookies.Remove("AccessToken");
                    var accessTokenCookie = new HttpCookie("AccessToken")
                    {
                        Expires = DateTime.Now.AddDays(-1) 
                    };
                    Response.Cookies.Add(accessTokenCookie);
                }

                if (Request.Cookies["RefreshToken"] != null)
                {
                    Response.Cookies.Remove("RefreshToken");
                    var refreshTokenCookie = new HttpCookie("RefreshToken")
                    {
                        Expires = DateTime.Now.AddDays(-1) 
                    };
                    Response.Cookies.Add(refreshTokenCookie);
                }

                TempData["LogoutMessage"] = "You have been logged out successfully.";
            }
            return RedirectToAction("Login");
        }

        public Users GetUserDetailsFromToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

            if (jsonToken == null)
                return null;

            var claims = jsonToken.Claims;

            var userDetails = new Users
            {
                UserName = claims.FirstOrDefault(c => c.Type == "unique_name")?.Value,
                Gender = claims.FirstOrDefault(c => c.Type == "Gender")?.Value,
                Age = int.TryParse(claims.FirstOrDefault(c => c.Type == "Age")?.Value, out var age) ? age : 0,
                Department = claims.FirstOrDefault(c => c.Type == "Department")?.Value,
                RoleId = claims.FirstOrDefault(c => c.Type == "role")?.Value,
            };

            return userDetails;
        }

        private void SetTokenCookie(string cookieName, string tokenValue, DateTime expiration)
        {
            var cookie = new HttpCookie(cookieName, tokenValue)
            {
                HttpOnly = true,
                Expires = expiration,
                Secure = Request.IsSecureConnection
            };
            Response.Cookies.Add(cookie);
        }

        private ActionResult RedirectToActionBasedOnResult(string roleId)
        {
            switch (roleId)
            {
                case "1":
                    return RedirectToAction("Dashboard", "Employee");
                case "2":
                    return RedirectToAction("Dashboard", "Accountant");
                case "3":
                    return RedirectToAction("Dashboard", "Admin");
                case "4":
                    return RedirectToAction("Dashboard", "Manager");
                default:
                    return RedirectToAction("Login", "User");
            }
        }
    }
}