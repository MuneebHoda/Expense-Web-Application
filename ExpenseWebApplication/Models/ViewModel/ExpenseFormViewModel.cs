using Newtonsoft.Json;
using System;
using System.Collections.Generic;

    namespace ExpenseWebApplication.Models.ViewModel
    {
        public class ExpenseFormViewModel
        {
            public int FormID { get; set; }
            public string Status { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime SubmittedDate { get; set; }
            public string ManagerName { get; set; }
            public string ManagerComments { get; set; }
            public string EmployeeName { get; set; }
            public List<ExpenseViewModel> Expenses { get; set; }
        }

        public class ExpenseViewModel
        {
            public int ExpenseID { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; }
            public DateTime Date { get; set; }
        }
    }

