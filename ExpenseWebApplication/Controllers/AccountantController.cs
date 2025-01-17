using Azure.Core;
using ExpenseWebApplication.Models;
using ExpenseWebApplication.Models.ViewModel;
using ExpenseWebApplication.Services;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ExpenseWebApplication.Controllers
{
    public class AccountantController : Controller
    {
        private readonly UserController _userController;
        private readonly ApprovalProcessServices _approvalProcessTable;
        private readonly ExpenseFormsServices _expenseFormsService;
        private AppDbContext _context;
        public AccountantController()
        {
            _userController = new UserController();
            _approvalProcessTable = new ApprovalProcessServices();
            _expenseFormsService = new ExpenseFormsServices();
            _context = new AppDbContext();
        }
        // GET: Accountant
        [Authorize]
        [RoleAuthorize("2")]
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

            var accountantId = User.Identity.GetUserId();
            Log.Information("Fetching expense forms for user: {UserId}", accountantId);
            var expenseForms = _expenseFormsService.GetExpenseFormsWithExpensesForAccountant(accountantId);
            var approvalProcesses = _context.ApprovalProcessTable
                                    .Where(ap => ap.AccountantID == accountantId && ap.ManagerApprovalStatus == "Approved") 
                                    .ToList();

            var approvalProcessViewModels = MapToViewModel(approvalProcesses);

            Console.WriteLine(expenseForms);
            var viewModel = new DashboardViewModel
            {
                UserName = userDetails.UserName,
                Age = userDetails.Age,
                Gender = userDetails.Gender,
                Department = userDetails.Department,
                ToPayCount = CountToPay(approvalProcessViewModels),
                PaidCount = CountPaid(approvalProcessViewModels),
                TotalPaidAmount = CalculateTotalPaidAmount(approvalProcessViewModels),
                ExpenseForms = expenseForms,
                
            };

            return View(viewModel);

        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadAccountantPendingExpenses()
        {
            var accountantId = User.Identity.GetUserId();
            var pendingExpenseForms = _approvalProcessTable.GetExpenseFormsForAccountant(accountantId, false); 

            if (pendingExpenseForms == null || !pendingExpenseForms.Any())
            {
                Log.Warning("No pending expense forms found for user: {UserId}", accountantId);
                return PartialView("~/Views/PartialViews/Accountant/_AccountantPendingExpenses.cshtml", new List<ApprovalProgressViewModel>());
            }

            Log.Information("Loading Partial Dashboard View for pending expenses");
            return PartialView("~/Views/PartialViews/Accountant/_AccountantPendingExpenses.cshtml", pendingExpenseForms);
        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadAccountantAcceptedExpenses()
        {
            var accountantId = User.Identity.GetUserId();
            var acceptedExpenseForms = _approvalProcessTable.GetExpenseFormsForAccountant(accountantId, true); 

            if (acceptedExpenseForms == null || !acceptedExpenseForms.Any())
            {
                Log.Warning("No accepted expense forms found for user: {UserId}", accountantId);
                return PartialView("~/Views/PartialViews/Accountant/_AccountantApprovedExpenses.cshtml", new List<ApprovalProgressViewModel>());
            }

            Log.Information("Loading Partial Dashboard View for accepted expenses");
            return PartialView("~/Views/PartialViews/Accountant/_AccountantApprovedExpenses.cshtml", acceptedExpenseForms);
        }


    [HttpPost]
    [Authorize]
    public ActionResult PayPendingExpenses()
    {
        try
        {
            // Read the request body to get the JSON data
            string requestBody;
            using (var reader = new StreamReader(Request.InputStream))
            {
                requestBody = reader.ReadToEnd();
            }

            // Deserialize the JSON data into a dictionary
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);
            if (data == null)
            {
                return Json(new { success = false, message = "Invalid request data." });
            }

            // Extract the expenseFormID from the dictionary
            if (!data.TryGetValue("expenseFormID", out string expenseFormIDString))
            {
                return Json(new { success = false, message = "Missing expenseFormID." });
            }

            // Parse the expenseFormID to an integer
            if (!int.TryParse(expenseFormIDString, out int expenseFormID))
            {
                return Json(new { success = false, message = "Invalid expenseFormID format." });
            }

            // Call the service to update the payment status
            var success = _approvalProcessTable.UpdatingPaymentStatus(expenseFormID);
            if (!success)
            {
                return Json(new { success = false, message = "Failed to update payment status." });
            }

            Log.Information("ExpenseFormID: {ExpenseFormID} was successfully approved by accountant.", expenseFormID);
            return Json(new { success = true, message = "Expense approved successfully." });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while approving an expense.");
            return Json(new { success = false, message = "An error occurred while processing the request." });
        }
    }
        private List<ApprovalProgressViewModel> MapToViewModel(IEnumerable<ApprovalProcessTable> approvalProcesses)
        {
            return approvalProcesses.Select(ap => new ApprovalProgressViewModel
            {
                TotalAmount = ap.ExpenseForm.TotalAmount,
                IsPaid = ap.IsPaid,
            }).ToList();
        }

        private int CountToPay(List<ApprovalProgressViewModel> approvalProcesses)
        {
            return approvalProcesses.Count(ap => !ap.IsPaid);
        }

        private int CountPaid(List<ApprovalProgressViewModel> approvalProcesses)
        {
            return approvalProcesses.Count(ap => ap.IsPaid);
        }

        private decimal CalculateTotalPaidAmount(List<ApprovalProgressViewModel> approvalProcesses)
        {
            return approvalProcesses
                .Where(ap => ap.IsPaid)
                .Sum(ap => ap.TotalAmount); 
        }
    }
}