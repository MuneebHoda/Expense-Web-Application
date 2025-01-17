using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseWebApplication.Models.ViewModel
{
    public class UpdateExpenseStatusViewModel
    {
        public int FormId { get; set; }
        public string Comments { get; set; }
        public string Action { get; set; }
    }

}