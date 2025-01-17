using ExpenseWebApplication.Models;
using ExpenseWebApplication.Models.ViewModel;
using ExpenseWebApplication.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace ExpenseWebApplication.Services
{
    public class ExpenseFormsServices : IExpenseFormsService
    {
        private readonly AppDbContext _context;

        public ExpenseFormsServices()
        {
            _context = new AppDbContext();
        }

        public bool SubmitExpenseForm(ExpenseFormSubmissionModel model, string userId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Log.Information("Starting expense form submission for user: {UserId}", userId);
                    var expenseForm = new ExpenseForms
                    {
                        EmployeeID = userId,
                        Status = "Pending",
                        SubmittedDate = DateTime.Now,
                        TotalAmount = model.Expenses.Sum(e => e.Amount),
                        ManagerID = GetManagerId(userId),
                        AccountantID = GetAccountantId(),
                    };

                    _context.ExpenseForms.Add(expenseForm);
                    _context.SaveChanges();
                    Log.Information("Expense form created successfully with FormID: {FormID}", expenseForm.FormID);
                    foreach (var expense in model.Expenses)
                    {
                        var expenseEntry = new Expenses
                        {
                            Date = expense.Date,
                            Currency = expense.Currency,
                            Amount = expense.Amount,
                            Description = expense.Description,
                            UserId = userId,
                            ExpenseFormID = expenseForm.FormID
                        };
                        _context.Expenses.Add(expenseEntry);
                    };
                    _context.SaveChanges();
                    Log.Information("All expenses added successfully for FormID: {FormID}", expenseForm.FormID);

                    var approvalProcess = new ApprovalProcessTable
                    {
                        ExpenseFormID = expenseForm.FormID,
                        ManagerApprovalStatus = "Pending",
                        AccountantApprovalStatus = "Pending",
                        IsPaid = false,
                        ManagerID = expenseForm.ManagerID,
                        AccountantID = expenseForm.AccountantID,
                    };
                    _context.ApprovalProcessTable.Add(approvalProcess);
                    _context.SaveChanges();
                    Log.Information("Approval process created successfully for FormID: {FormID}", expenseForm.FormID);

                    transaction.Commit();
                    Log.Information("Transaction committed successfully for FormID: {FormID}", expenseForm.FormID);

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occurred while submitting expense form for user: {UserId}", userId);
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public List<ExpenseFormViewModel> GetExpenseFormsWithExpensesForUser(string userId,string status = null)
        {
            var expenseFormViewModels = new List<ExpenseFormViewModel>();

            try
            {
                var query = _context.ExpenseForms.AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(ef => ef.EmployeeID == userId && ef.Status == status);
                }
                else
                {
                    query = query.Where(ef => ef.EmployeeID == userId);
                }

                var expenseForms = query.ToList();

                if (!expenseForms.Any())
                {
                    Log.Warning("No expense forms found for user: {UserId}", userId);
                    return expenseFormViewModels; 
                }

                foreach (var expenseForm in expenseForms)
                {
                    var expenses = _context.Expenses
                                           .Where(e => e.ExpenseFormID == expenseForm.FormID)
                                           .Select(e => new ExpenseViewModel
                                           { 
                                               ExpenseID = e.ExpenseID,
                                               Description = e.Description,
                                               Amount = e.Amount,
                                               Currency = e.Currency,
                                               Date = e.Date,
                                           })
                                           .ToList();

                    var manager = _context.Users.FirstOrDefault(u => u.Id == expenseForm.ManagerID);

                    var viewModel = new ExpenseFormViewModel
                    {
                        FormID = expenseForm.FormID,
                        Status = expenseForm.Status,
                        TotalAmount = expenseForm.TotalAmount,
                        SubmittedDate = expenseForm.SubmittedDate,
                        ManagerName = manager?.UserName,
                        ManagerComments = expenseForm?.ManagerComments,
                        Expenses = expenses,
                    };

                    expenseFormViewModels.Add(viewModel);
                }

                Log.Information("Successfully fetched {Count} expense forms for user: {UserId}", expenseFormViewModels.Count, userId);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving expense forms for user: {UserId}", userId);
            }
            return expenseFormViewModels;
        }

        public List<ExpenseFormViewModel> GetExpenseFormsWithExpensesForManager(string managerId, string status = null)
        {
            var expenseFormViewModels = new List<ExpenseFormViewModel>();

            try
            {
                var query = _context.ExpenseForms.AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(ef => ef.ManagerID == managerId && ef.Status == status);
                }
                else
                {
                    query = query.Where(ef => ef.ManagerID == managerId);
                }

                var expenseForms = query.ToList();

                if (!expenseForms.Any())
                {
                    Log.Warning("No expense forms found for user: {UserId}", managerId);
                    return expenseFormViewModels;
                }

                foreach (var expenseForm in expenseForms)
                {
                    var expenses = _context.Expenses
                                           .Where(e => e.ExpenseFormID == expenseForm.FormID)
                                           .Select(e => new ExpenseViewModel
                                           {
                                               ExpenseID = e.ExpenseID,
                                               Description = e.Description,
                                               Amount = e.Amount,
                                               Currency = e.Currency,
                                               Date = e.Date,
                                           })
                                           .ToList();

                    var employee = _context.Users.FirstOrDefault(u => u.Id == expenseForm.EmployeeID);

                    var viewModel = new ExpenseFormViewModel
                    {
                        FormID = expenseForm.FormID,
                        Status = expenseForm.Status,
                        TotalAmount = expenseForm.TotalAmount,
                        SubmittedDate = expenseForm.SubmittedDate,
                        EmployeeName = employee?.UserName,
                        Expenses = expenses,
                    };

                    expenseFormViewModels.Add(viewModel);
                }

                Log.Information("Successfully fetched {Count} expense forms for user: {UserId}", expenseFormViewModels.Count, managerId);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving expense forms for user: {UserId}", managerId);
            }
            return expenseFormViewModels;
        }

        public List<ExpenseFormViewModel> GetExpenseFormsWithExpensesForAccountant(string accountantId)
        {
            var expenseFormViewModels = new List<ExpenseFormViewModel>();

            try
            {
                var query = _context.ExpenseForms
                                    .Where(ef => ef.AccountantID == accountantId)
                                    .AsQueryable();

                var expenseForms = query.ToList();

                if (!expenseForms.Any())
                {
                    Log.Warning("No expense forms found for accountant: {AccountantId}", accountantId);
                    return expenseFormViewModels;
                }

                foreach (var expenseForm in expenseForms)
                {
                    var expenses = _context.Expenses
                                           .Where(e => e.ExpenseFormID == expenseForm.FormID)
                                           .Select(e => new ExpenseViewModel
                                           {
                                               ExpenseID = e.ExpenseID,
                                               Description = e.Description,
                                               Amount = e.Amount,
                                               Currency = e.Currency,
                                               Date = e.Date,
                                           })
                                           .ToList();

                    var employee = _context.Users.FirstOrDefault(u => u.Id == expenseForm.EmployeeID);

                    var viewModel = new ExpenseFormViewModel
                    {
                        FormID = expenseForm.FormID,
                        Status = expenseForm.Status,
                        TotalAmount = expenseForm.TotalAmount,
                        SubmittedDate = expenseForm.SubmittedDate,
                        EmployeeName = employee?.UserName,
                        Expenses = expenses,
                    };

                    expenseFormViewModels.Add(viewModel);
                }

                Log.Information("Successfully fetched {Count} expense forms for accountant: {AccountantId}", expenseFormViewModels.Count, accountantId);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving expense forms for accountant: {AccountantId}", accountantId);
            }
            return expenseFormViewModels;
        }

        public List<ExpenseFormViewModel> GetAllExpenseFormsWithExpensesForAdmin()
        {
            var expenseFormViewModels = new List<ExpenseFormViewModel>();

            try
            {
                var expenseForms = _context.ExpenseForms.ToList();

                if (!expenseForms.Any())
                {
                    Log.Warning("No expense forms found.");
                    return expenseFormViewModels;
                }

                foreach (var expenseForm in expenseForms)
                {
                    var expenses = _context.Expenses
                                           .Where(e => e.ExpenseFormID == expenseForm.FormID)
                                           .Select(e => new ExpenseViewModel
                                           {
                                               ExpenseID = e.ExpenseID,
                                               Description = e.Description,
                                               Amount = e.Amount,
                                               Currency = e.Currency,
                                               Date = e.Date,
                                           })
                                           .ToList();

                    var employee = _context.Users.FirstOrDefault(u => u.Id == expenseForm.EmployeeID);

                    var viewModel = new ExpenseFormViewModel
                    {
                        FormID = expenseForm.FormID,
                        Status = expenseForm.Status,
                        TotalAmount = expenseForm.TotalAmount,
                        SubmittedDate = expenseForm.SubmittedDate,
                        EmployeeName = employee?.UserName,
                        Expenses = expenses,
                    };

                    expenseFormViewModels.Add(viewModel);
                }

                Log.Information("Successfully fetched {Count} expense forms.", expenseFormViewModels.Count);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving expense forms.");
            }

            return expenseFormViewModels;
        }



        public string UpdateExpenseStatusFunction(int formId, string comments, string action)
        {
            try
            {
                var expenseForm = _context.ExpenseForms.Find(formId);
                if (expenseForm == null)
                {
                    Log.Warning("Expense form with ID {FormId} not found.", formId);
                    return "Expense form not found.";
                }

                if (action.ToLower() == "approve")
                {
                    expenseForm.Status = "Approved";
                    expenseForm.ApprovalDate = DateTime.UtcNow;
                }
                else if (action.ToLower() == "reject")
                {
                    expenseForm.Status = "Rejected";
                    expenseForm.ManagerComments = comments;
                    expenseForm.ApprovalDate = null; 
                }
                else
                {
                    Log.Warning("Invalid action received: {Action}.", action);
                    return "Invalid action.";
                }

                var approvalProcess = _context.ApprovalProcessTable
                    .FirstOrDefault(ap => ap.ExpenseFormID == formId);

                if (approvalProcess != null)
                {
                    approvalProcess.ManagerApprovalStatus = action.ToLower() == "approve" ? "Approved" : "Rejected";
                    approvalProcess.ManagerComments = comments;
                    approvalProcess.ManagerApprovalDate = action.ToLower() == "approve" ? DateTime.UtcNow : (DateTime?)null;

                    approvalProcess.IsPaid = false; 
                }

                _context.SaveChanges();

                Log.Information("Expense {Action}d successfully for form ID {FormId}.", action, formId);
                return $"Expense {action} successfully.";
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex, "An error occurred while updating the expense status for form ID {FormId}.", formId);
                return "An error occurred while updating the expense status. Please try again.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while updating the expense status for form ID {FormId}.", formId);
                return "An unexpected error occurred. Please try again.";
            }
        }

        public bool UpdateExpenseAndExpenseForms(List<ExpenseItemViewModel> changes)
        {
            try
            {
                if (changes == null || !changes.Any())
                {
                    Log.Warning("Invalid changes provided to UpdateExpenseAndExpenseForms.");
                    return false;
                }

                int expenseFormID = changes.First().ExpenseFormID;
                var expenseForm = _context.ExpenseForms.FirstOrDefault(f => f.FormID == expenseFormID);

                if (expenseForm == null)
                {
                    Log.Warning($"Expense form with ID {expenseFormID} not found.");
                    return false;
                }

                decimal totalAmount = 0;
                foreach (var change in changes)
                {
                    switch (change.Action.ToLower())
                    {
                        case "added":
                            var newExpense = new Expenses
                            {
                                ExpenseID = change.Data.ExpenseID,
                                Description = change.Data.Description,
                                Amount = change.Data.Amount,
                                Currency = change.Data.Currency,
                                Date = change.Data.ExpenseDate,
                                ExpenseFormID = expenseFormID
                            };

                            _context.Expenses.Add(newExpense);
                            totalAmount += newExpense.Amount;
                            Log.Information("Added new expense: {@NewExpense}", newExpense);
                            break;

                        case "updated":
                            var existingExpense = _context.Expenses.FirstOrDefault(e => e.ExpenseID == change.Data.ExpenseID);
                            if (existingExpense != null)
                            {
                                existingExpense.Description = change.Data.Description;
                                existingExpense.Amount = change.Data.Amount;
                                existingExpense.Currency = change.Data.Currency;
                                existingExpense.Date = change.Data.ExpenseDate;

                                totalAmount += existingExpense.Amount;
                                Log.Information("Updated expense: {@UpdatedExpense}", existingExpense);
                            }
                            else
                            {
                                Log.Warning("Attempted to update non-existing expense with ID {ExpenseID}.", change.Data.ExpenseID);
                            }
                            break;

                        case "deleted":
                            var expenseToDelete = _context.Expenses.FirstOrDefault(e => e.ExpenseID == change.Data.ExpenseID);
                            if (expenseToDelete != null)
                            {
                                _context.Expenses.Remove(expenseToDelete);
                                Log.Information("Deleted expense: {@DeletedExpense}", expenseToDelete);
                            }
                            else
                            {
                                Log.Warning("Attempted to delete non-existing expense with ID {ExpenseID}.", change.Data.ExpenseID);
                            }
                            break;

                        default:
                            Log.Warning("Unrecognized action '{Action}' for expense.", change.Action);
                            break;
                    }
                }

                expenseForm.Status = "Pending";
                expenseForm.SubmittedDate = DateTime.Now;
                expenseForm.TotalAmount = totalAmount;
                Log.Information("Updated expense form ID {ExpenseFormID} with total amount: {TotalAmount}", expenseFormID, totalAmount);

                var approvalProcess = _context.ApprovalProcessTable.FirstOrDefault(ap => ap.ExpenseFormID == expenseFormID);
                if (approvalProcess != null)
                {
                    approvalProcess.ManagerApprovalStatus = "Pending";
                    approvalProcess.ManagerComments = null;
                    Log.Information("Updated approval process for expense form ID {ExpenseFormID}.", expenseFormID);
                }
                else
                {
                    Log.Warning("Approval process for expense form ID {ExpenseFormID} not found.", expenseFormID);
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating expenses and expense forms.");
                return false;
            }
        }


        private string GetManagerId(string userId)
        {
            try
            {
                Log.Information("Fetching ManagerID for user: {UserId}", userId);
                var managerId = _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.ManagerId)
                    .FirstOrDefault();
                Log.Information("ManagerID: {ManagerID} fetched successfully for user: {UserId}", managerId, userId);
                return managerId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching ManagerID for user: {UserId}", userId);
                throw;
            }
        }

        private string GetAccountantId()
        {
            try
            {
                Log.Information("Fetching default AccountantID");
                string accountantId = "46aa98a1-98bb-4335-87c4-b167a1c51ecc";
                Log.Information("AccountantID: {AccountantID} fetched successfully", accountantId);
                return accountantId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching AccountantID");
                throw;
            }
        }
    }
}