using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseWebApplication.Models.ViewModel
{
    public class ExpenseItemViewModel
    {
        public int ExpenseFormID { get; set; } 
        public string Action { get; set; }     
        public ExpenseDataModel Data { get; set; }
    }

    public class ExpenseDataModel
    {
        public int ExpenseID { get; set; }   
        public string Currency { get; set; }    
        public string Description { get; set; }  
        public DateTime ExpenseDate { get; set; } 
        public decimal Amount { get; set; }   
    }
}