using ExpenseWebApplication.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseWebApplication.Models
{
    public class Expenses
    {
        [Key]
        public int ExpenseID { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }

        //Foreign Keys
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("ExpenseForm")]
        public int ExpenseFormID { get; set; }

        //Navigation Properties
        public virtual Users User { get; set; }
        public virtual ExpenseForms ExpenseForm { get; set; }

    }
}