using ExpenseWebApplication.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseWebApplication.Services.Interfaces
{
    public interface IExpenseFormsService
    {
        bool SubmitExpenseForm(ExpenseFormSubmissionModel model, string userId);
    }
}