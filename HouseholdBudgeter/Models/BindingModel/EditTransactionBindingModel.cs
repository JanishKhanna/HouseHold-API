using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.BindingModel
{
    public class EditTransactionBindingModel
    {
        public string Title { get; set; }        
        public string Description { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public decimal Amount { get; set; }
    }
}