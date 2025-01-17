using ExpenseWebApplication.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseWebApplication.Models { 
    public class ApprovalProcessTable{

        [Key]
        public int ApprovalID {  get; set; }

        [Required]
        public string ManagerApprovalStatus { get; set; }
        public string ManagerComments { get; set; }
        public DateTime? ManagerApprovalDate { get; set; }
        [Required]
        public string AccountantApprovalStatus { get; set; }
        public DateTime? AccountantApprovalDate { get; set; }
        public bool IsPaid { get; set; }

        //Foreign Keys
        [ForeignKey("Manager")]
        public string ManagerID { get; set; }

        [ForeignKey("Accountant")]
        public string AccountantID { get; set; }

        [ForeignKey("ExpenseForm")]
        public int ExpenseFormID { get; set; }

        //Navigation Properties
        public virtual ExpenseForms ExpenseForm { get; set; }
        public virtual Users Manager {  get; set; }
        public virtual Users Accountant { get; set; }
    }
}