using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseWebApplication.Models.ViewModel
{
    public class ApprovalProgressViewModel
    {
        public int FormID { get; set; }
        public DateTime SubmittedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string ManagerID { get; set; }
        public string ManagerUserName { get; set; }
        public string AccountantID { get; set; }
        public string AccountantUserName { get; set; }
        public DateTime? ManagerApprovalDate { get; set; }
        public DateTime? AccountantApprovalDate { get; set; }
        public string ManagerApprovalStatus { get; set; }
        public string AccountantApprovalStatus { get; set; }
        public bool IsPaid { get; set; }
        public List<Expenses> Expenses { get; set; }
    }
}