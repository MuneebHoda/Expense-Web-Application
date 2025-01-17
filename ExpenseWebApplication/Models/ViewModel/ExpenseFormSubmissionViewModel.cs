using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ExpenseWebApplication.Models.ViewModel {
    public class ExpenseFormSubmissionModel
    { 
        public List<ExpenseItemModel> Expenses { get; set; }
    }
    public class ExpenseItemModel
    {
        [JsonProperty("expenseDate")]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}