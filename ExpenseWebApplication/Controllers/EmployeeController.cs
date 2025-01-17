using ExpenseWebApplication.Models.ViewModel;
using ExpenseWebApplication.Services;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ExpenseWebApplication.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserController _userController;
        private readonly ExpenseFormsServices _expenseFormsService;

        public EmployeeController()
        {
            _userController = new UserController();
            _expenseFormsService = new ExpenseFormsServices();
        }

        // GET: Employee
        [Authorize]
        [RoleAuthorize("1")]
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
            var expenseForms = _expenseFormsService.GetExpenseFormsWithExpensesForUser(userId);
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
        public ActionResult LoadEmployeeDashboardExpenses()
        {
            var userId = User.Identity.GetUserId();
            var expenseForms = _expenseFormsService.GetExpenseFormsWithExpensesForUser(userId);

            if (expenseForms == null || !expenseForms.Any())
            {
                Log.Warning("No expense forms found for user: {UserId}", userId);
                return PartialView("~/Views/PartialViews/Employee/_DashboardContent.cshtml", new List<ExpenseFormViewModel>());
            }
            Log.Information("Loading Partial Dashboard View");
            return PartialView("~/Views/PartialViews/Employee/_DashboardContent.cshtml", expenseForms);
        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadEmployeePendingExpenses()
        {
            var userId = User.Identity.GetUserId();
            var pendingForms = _expenseFormsService.GetExpenseFormsWithExpensesForUser(userId, "Pending");
            return PartialView("~/Views/PartialViews/Employee/_EmployeePendingExpenses.cshtml", pendingForms);
        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadEmployeeAcceptedExpenses()
        {
            var userId = User.Identity.GetUserId();
            var acceptedForms = _expenseFormsService.GetExpenseFormsWithExpensesForUser(userId, "Approved");
            return PartialView("~/Views/PartialViews/Employee/_EmployeeAcceptedExpenses.cshtml", acceptedForms);
        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadEmployeeRejectedExpenses()
        {
            var userId = User.Identity.GetUserId();
            var rejectedForms = _expenseFormsService.GetExpenseFormsWithExpensesForUser(userId, "Rejected");
            return PartialView("~/Views/PartialViews/Employee/_EmployeeRejectedExpenses.cshtml", rejectedForms);
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
            return PartialView("~/Views/PartialViews/Employee/_ApprovalProcess.cshtml", approvalProgress);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditExpensesAndExpenseForm()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var dataRead = Request.Form["trackedChanges"];
                    var changes = JsonConvert.DeserializeObject<List<ExpenseItemViewModel>>(dataRead);

                    if (changes == null || !changes.Any())
                    {
                        Log.Warning("Invalid request body in EditExpensesAndExpenseForm.");
                        return Json(new { success = false, message = "Invalid request body." });
                    }

                    Log.Information("Changes received for processing: {@Changes}", changes);
                    var result = _expenseFormsService.UpdateExpenseAndExpenseForms(changes);

                    if (result)
                    {
                        Log.Information("Expenses and expense form updated successfully.");
                        return Json(new { success = true, message = "Expenses updated successfully." });
                    }
                    else
                    {
                        Log.Warning("Failed to update expenses and expense form.");
                        return Json(new { success = false, message = "Failed to update expenses. Please try again." });
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while updating expenses.");
                return Json(new { success = false, message = "An unexpected error occurred. Please try again." });
            }
        }
    }
}