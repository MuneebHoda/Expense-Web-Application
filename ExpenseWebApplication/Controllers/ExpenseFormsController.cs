using ExpenseWebApplication.Models.ViewModel;
using ExpenseWebApplication.Services;
using ExpenseWebApplication.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Serilog;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseWebApplication.Controllers
{
    public class ExpenseFormsController : Controller
    {
        private readonly IExpenseFormsService _expenseFormsService;
        private AppDbContext _context;
        public ExpenseFormsController()
        {
            _expenseFormsService = new ExpenseFormsServices();
            _context = new AppDbContext();
        }
        public ExpenseFormsController(IExpenseFormsService expenseFormsService)
        {
            _expenseFormsService = expenseFormsService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SubmitExpense()
        {
            try
            {
                var expenseJson = Request.Form["Expenses"];
                var expenses = JsonConvert.DeserializeObject<List<ExpenseItemModel>>(expenseJson);

                if (expenses == null || !expenses.Any())
                {
                    Log.Warning("No expenses found in JSON data for expense form submission.");
                    return Json(new { success = false, message = "No expenses found." });
                }

                var model = new ExpenseFormSubmissionModel { Expenses = expenses };

                if (!ModelState.IsValid)
                {
                    Log.Warning("Invalid model state for expense form submission.");
                    return Json(new { success = false, message = "Invalid data submitted." });
                }

                var userId = User.Identity.GetUserId();
                var result = _expenseFormsService.SubmitExpenseForm(model, userId);

                if (result)
                {
                    Log.Information("Expense form submitted successfully for user: {UserId}", userId);
                    return Json(new { success = true, message = "Expense form submitted successfully." });
                }
                else
                {
                    Log.Error("Error occurred while submitting expense form for user: {UserId}", userId);
                    return Json(new { success = false, message = "Failed to submit expense form." });
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while submitting expense form.");
                return Json(new { success = false, message = "An unexpected error occurred. Please try again." });
            }
        }

    }
}