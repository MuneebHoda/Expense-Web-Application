using ExpenseWebApplication.Models.ViewModel;
using ExpenseWebApplication.Services;
using ExpenseWebApplication.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExpenseWebApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserController _userController;
        private readonly ExpenseFormsServices _expenseFormsService;
        private readonly ApprovalProcessServices _approvalProcessService;

        public AdminController()
        {
            _userController = new UserController();
            _expenseFormsService = new ExpenseFormsServices();
            _approvalProcessService = new ApprovalProcessServices();
        }
        // GET: Admin
        [Authorize]
        [RoleAuthorize("3")]
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

            var adminId = User.Identity.GetUserId();
            Log.Information("Fetching expense forms for user: {UserId}", adminId);
            var expenseForms = _expenseFormsService.GetAllExpenseFormsWithExpensesForAdmin();
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
        public ActionResult LoadChartsPartialView()
        {
            return PartialView("~/Views/PartialViews/Admin/_AdminChartsDashboard.cshtml");
        }

        [HttpGet]
        [Authorize]
        public ActionResult LoadTransactionDashboard()
        {
            var expenseForms = _approvalProcessService.GetAll();
            return PartialView("~/Views/PartialViews/Admin/_AdminTransactionTable.cshtml",expenseForms);
        }

    }
}