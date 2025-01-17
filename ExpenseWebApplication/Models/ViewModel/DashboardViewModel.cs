using ExpenseWebApplication.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseWebApplication.Models.ViewModel
{
    public class DashboardViewModel
    {
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public List<ExpenseFormViewModel> ExpenseForms { get; set; }

        public int ToPayCount { get; set; } 
        public int PaidCount { get; set; }    
        public decimal TotalPaidAmount { get; set; }
    }
}