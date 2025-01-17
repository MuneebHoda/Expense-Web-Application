using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.UI.WebControls;
namespace ExpenseWebApplication.Models
{
    public class Users : IdentityUser
    {
        public int UserCode { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public string RoleId { get; set; }
        public string ManagerId { get; set; }

        //Navigation Properties
        public virtual Roles Role { get; set; }
        public virtual Users Manager { get; set; }
    }
}