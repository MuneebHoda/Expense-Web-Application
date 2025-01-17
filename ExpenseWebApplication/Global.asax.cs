using Serilog;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Web.Helpers;

namespace ExpenseWebApplication
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new AuthorizeAttribute());
            InitializeDatabase();
            ConfigureLogging();
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }

        protected void Application_End()
        {
            Log.Information("Logger Closing");
            LogService.ShutdownLogger();
        }

        private void InitializeDatabase()
        {
            using (var context = new AppDbContext())
            {
                new AppDbInitializer().Seed(context);
            }
        }

        private void ConfigureLogging()
        {
            try
            {
                LogService.ConfigureLogging();
                Log.Information("Application started successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting up logging: " + ex.Message);
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var token = HttpContext.Current.Request.Cookies["AccessToken"]?.Value;

            if (token != null )
            {
                try
                {
                    var jwtSecret = System.Configuration.ConfigurationManager.AppSettings["JwtSecret"];
                    if (string.IsNullOrEmpty(jwtSecret))
                    {
                        Log.Error("JWT Secret is missing in configuration.");
                        return;
                    }
                    var key = Encoding.ASCII.GetBytes(jwtSecret);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero 
                    };

                    SecurityToken validatedToken;
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                    HttpContext.Current.User = principal;

                    Log.Information("Token successfully validated for user: {User}", principal.Identity.Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occurred during token validation");
                    HttpContext.Current.User = null;
                }
            }
        }
    }
}
