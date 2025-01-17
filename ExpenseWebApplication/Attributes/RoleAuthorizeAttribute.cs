using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class RoleAuthorizeAttribute : AuthorizeAttribute
{
    private readonly string[] allowedRoles;

    public RoleAuthorizeAttribute(params string[] roles)
    {
        this.allowedRoles = roles;
    }

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var roleId = httpContext.Request.Cookies["AccessToken"] != null
            ? GetUserRoleId(httpContext.Request.Cookies["AccessToken"].Value)
            : null;

        return allowedRoles.Contains(roleId);
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
        {
            { "controller", "User" },
            { "action", "Login" }
        });
    }

    private string GetUserRoleId(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

        if (jsonToken == null)
            return null;

        return jsonToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
    }
}
