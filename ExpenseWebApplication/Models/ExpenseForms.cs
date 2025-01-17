using ExpenseWebApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExpenseWebApplication.Models
{
    public class ExpenseForms
    {
        [Key]
        public int FormID { get; set; }

        public string Status { get; set; }  // Pending, Approved, Rejected, Paid
        public decimal TotalAmount { get; set; }
        public string ManagerComments { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? PaymentTime { get; set; }

        // Foreign Keys
        [ForeignKey("Employee")]
        public string EmployeeID { get; set; }

        [ForeignKey("Manager")]
        public string ManagerID { get; set; }

        [ForeignKey("Accountant")]
        public string AccountantID { get; set; }

        // Navigation Properties
        public virtual Users Employee { get; set; }
        public virtual Users Manager { get; set; }
        public virtual Users Accountant { get; set; }
        public virtual ICollection<Expenses> Expenses { get; set; }
    }
}