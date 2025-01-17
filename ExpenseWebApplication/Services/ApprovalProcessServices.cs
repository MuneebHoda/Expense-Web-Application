using ExpenseWebApplication.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System;

namespace ExpenseWebApplication.Services
{
    public class ApprovalProcessServices
    {
        private readonly AppDbContext _context;

        public ApprovalProcessServices()
        {
            _context = new AppDbContext();
        }

        public List<ApprovalProgressViewModel> GetApprovalProgressForUser(string userId)
        {
            var approvalProgressList = new List<ApprovalProgressViewModel>();

            try
            {
                var expenseForms = _context.ExpenseForms
                                            .Where(exp => exp.EmployeeID == userId)
                                            .ToList();

                if (!expenseForms.Any())
                {
                    Log.Warning("No expense forms found for user: {UserId}", userId);
                    return approvalProgressList;
                }

                foreach (var expense in expenseForms)
                {
                    var approval = _context.ApprovalProcessTable
                                           .FirstOrDefault(a => a.ExpenseFormID == expense.FormID);

                    if (approval != null)
                    {
                        approvalProgressList.Add(new ApprovalProgressViewModel
                        {
                            FormID = expense.FormID,
                            SubmittedAt = expense.SubmittedDate,
                            TotalAmount = expense.TotalAmount,
                            ManagerID = approval.ManagerID,
                            ManagerUserName = approval.Manager.UserName,
                            AccountantID = approval.AccountantID,
                            AccountantUserName = approval.Accountant.UserName,
                            ManagerApprovalDate = approval.ManagerApprovalDate,
                            AccountantApprovalDate = approval.AccountantApprovalDate,
                            ManagerApprovalStatus = approval.ManagerApprovalStatus,
                            AccountantApprovalStatus = approval.AccountantApprovalStatus,
                            IsPaid = approval.IsPaid
                        });
                    }
                }

                Log.Information("Successfully fetched approval progress for user: {UserId}", userId);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving approval progress for user: {UserId}", userId);
            }

            return approvalProgressList;
        }

        public List<ApprovalProgressViewModel> GetExpenseFormsForAccountant(string accountantId, bool isPaid)
        {
            var expenseForms = new List<ApprovalProgressViewModel>();

            try
            {
                var approvalProcesses = _context.ApprovalProcessTable
                    .Where(ap => ap.AccountantID == accountantId &&
                                 ap.IsPaid == isPaid && // Use the isPaid parameter here
                                 ap.ManagerApprovalStatus == "Approved")
                    .ToList();

                if (!approvalProcesses.Any())
                {
                    Log.Warning("No approval processes found for accountant: {AccountantId}", accountantId);
                    return expenseForms;
                }

                foreach (var approval in approvalProcesses)
                {
                    var expenseForm = _context.ExpenseForms
                        .FirstOrDefault(exp => exp.FormID == approval.ExpenseFormID);

                    if (expenseForm != null)
                    {
                        var expenses = _context.Expenses
                            .Where(e => e.ExpenseFormID == expenseForm.FormID)
                            .ToList();

                        expenseForms.Add(new ApprovalProgressViewModel
                        {
                            FormID = approval.ExpenseFormID,
                            SubmittedAt = expenseForm.SubmittedDate,
                            TotalAmount = expenseForm.TotalAmount,
                            ManagerID = approval.ManagerID,
                            ManagerUserName = approval.Manager.UserName,
                            AccountantID = approval.AccountantID,
                            AccountantUserName = approval.Accountant.UserName,
                            ManagerApprovalDate = approval.ManagerApprovalDate,
                            AccountantApprovalDate = approval.AccountantApprovalDate,
                            ManagerApprovalStatus = approval.ManagerApprovalStatus,
                            AccountantApprovalStatus = approval.AccountantApprovalStatus,
                            IsPaid = approval.IsPaid,
                            Expenses = expenses
                        });
                    }
                }

                Log.Information("Successfully fetched expense forms for accountant: {AccountantId}", accountantId);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving expense forms for accountant: {AccountantId}", accountantId);
            }

            return expenseForms;
        }

        public List<ApprovalProgressViewModel> GetAll()
        {
            var expenseForms = new List<ApprovalProgressViewModel>();

            try
            {
                var approvalProcesses = _context.ApprovalProcessTable
                    .ToList();

                if (!approvalProcesses.Any())
                {
                    Log.Warning("No approval processes found.");
                    return expenseForms;
                }

                foreach (var approval in approvalProcesses)
                {
                    var expenseForm = _context.ExpenseForms
                        .FirstOrDefault(exp => exp.FormID == approval.ExpenseFormID);

                    if (expenseForm != null)
                    {
                        var expenses = _context.Expenses
                            .Where(e => e.ExpenseFormID == expenseForm.FormID)
                            .ToList();

                        expenseForms.Add(new ApprovalProgressViewModel
                        {
                            FormID = approval.ExpenseFormID,
                            SubmittedAt = expenseForm.SubmittedDate,
                            TotalAmount = expenseForm.TotalAmount,
                            ManagerID = approval.ManagerID,
                            ManagerUserName = approval.Manager.UserName,
                            AccountantID = approval.AccountantID,
                            AccountantUserName = approval.Accountant.UserName,
                            ManagerApprovalDate = approval.ManagerApprovalDate,
                            AccountantApprovalDate = approval.AccountantApprovalDate,
                            ManagerApprovalStatus = approval.ManagerApprovalStatus,
                            AccountantApprovalStatus = approval.AccountantApprovalStatus,
                            IsPaid = approval.IsPaid,
                            Expenses = expenses
                        });
                    }
                }

                Log.Information("Successfully fetched all expense forms.");
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Exception occurred while retrieving all expense forms.");
            }

            return expenseForms;
        }


        public bool UpdatingPaymentStatus(int expenseFormID)
        {
            try
            {
                var approvalProcess = _context.ApprovalProcessTable.FirstOrDefault(ap => ap.ExpenseFormID == expenseFormID);

                if (approvalProcess == null)
                {
                    Log.Warning("Approval process not found for ExpenseFormID: {ExpenseFormID}", expenseFormID);
                    return false;
                }

                approvalProcess.AccountantApprovalStatus = "Approved";
                approvalProcess.AccountantApprovalDate = DateTime.Now;
                approvalProcess.IsPaid = true;

                var expenseForm = _context.ExpenseForms.FirstOrDefault(exp => exp.FormID == expenseFormID);

                if (expenseForm == null)
                {
                    Log.Warning("Expense form not found for ExpenseFormID: {ExpenseFormID}", expenseFormID);
                    return false;
                }

                expenseForm.PaymentTime = DateTime.Now;

                _context.SaveChanges();

                Log.Information("Payment status successfully updated for ExpenseFormID: {ExpenseFormID}", expenseFormID);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while updating payment status for ExpenseFormID: {ExpenseFormID}", expenseFormID);
                return false;
            }
        }
    }
}
