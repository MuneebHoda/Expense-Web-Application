using ExpenseWebApplication.Models.ViewModel;
using ExpenseWebApplication.Services;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ExpenseWebApplication.Controllers
{
    public class ManagerController : Controller
    {
        private readonly UserController _userController;
        private readonly ExpenseFormsServices _expenseFormsService;
        public ManagerController()
        {
            _userController = new UserController();
            _expenseFormsService = new ExpenseFormsServices();
        }

        // GET: Manager
        [Authorize]
        [RoleAuthorize("4")]
        public ActionResult Dashboard()
        {
            var accessTokenCookie = Request.Cookies["AccessToken"];
            var accessToken = accessTokenCookie?.Value;

            var userDetails = _userController.GetUserDetailsFromToken(accessToken);

            if (userDetails == null)
            {
                ViewBag.ShowNavbar = true;
                return View();
            }

            var userId = User.Identity.GetUserId();
            Log.Information("Fetching expense forms for user: {UserId}", userId);
            var expenseForms = _expenseFormsService.GetExpenseFormsWithExpensesForManager(userId);
            var viewModel = new DashboardViewModel
            {
                UserName = userDetails.UserName,
                Age = userDetails.Age,
                Gender = userDetails.Gender,
                Department = userDetails.Department,
                ExpenseForms = expenseForms,
            };

            return View(viewModel);

        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadManagerPendingExpenses()
        {
            var userId = User.Identity.GetUserId();
            var pendingForms = _expenseFormsService.GetExpenseFormsWithExpensesForManager(userId, "Pending");
            return PartialView("~/Views/PartialViews/Manager/_ManagerPendingExpenses.cshtml", pendingForms);
        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadManagerAcceptedExpenses()
        {
            var userId = User.Identity.GetUserId();
            var acceptedForms = _expenseFormsService.GetExpenseFormsWithExpensesForManager(userId, "Approved");
            return PartialView("~/Views/PartialViews/Manager/_ManagerAcceptedExpenses.cshtml", acceptedForms);
        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadManagerRejectedExpenses()
        {
            var userId = User.Identity.GetUserId();
            var rejectedForms = _expenseFormsService.GetExpenseFormsWithExpensesForManager(userId, "Rejected");
            return PartialView("~/Views/PartialViews/Manager/_ManagerRejectedExpenses.cshtml", rejectedForms);
        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadApprovalProgressExpenses()
        {
            var userId = User.Identity.GetUserId();
            var approvalProcessService = new ApprovalProcessServices();
            var approvalProgress = approvalProcessService.GetApprovalProgressForUser(userId);

            if (approvalProgress == null || !approvalProgress.Any())
            {
                Log.Warning("No approval progress found for user: {UserId}", userId);
                return PartialView("~/Views/PartialViews/Employee/_ApprovalProcess.cshtml", new List<ApprovalProgressViewModel>());
            }

            Log.Information("Loading Approval Progress View with {Count} entries", approvalProgress.Count);
            return PartialView("~/Views/PartialViews/Manager/_ApprovalProcess.cshtml", approvalProgress);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateExpenseStatus()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var dataRead = Request.Form["data"];
                    var model = JsonConvert.DeserializeObject<UpdateExpenseStatusViewModel>(dataRead);

                    if (model == null)
                    {
                        Log.Warning("Invalid request body in UpdateExpenseStatus.");
                        return Json(new { success = false, message = "Invalid request body." });
                    }

                    var resultMessage = _expenseFormsService.UpdateExpenseStatusFunction(model.FormId, model.Comments, model.Action);

                    if (string.IsNullOrEmpty(resultMessage))
                    {
                        Log.Warning("Failed to update the status for FormId: {FormId}", model.FormId);
                        return Json(new { success = false, message = "Failed to update status." });
                    }

                    Log.Information("Status updated successfully for FormId: {FormId}", model.FormId);
                    return Json(new { success = true, message = "Status updated successfully." });
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while updating expense status.");
                return Json(new { success = false, message = "An unexpected error occurred. Please try again." });
            }
        }

    }
}