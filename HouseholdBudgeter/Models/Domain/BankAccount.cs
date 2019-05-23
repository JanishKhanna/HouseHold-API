using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Domain
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal Balance { get; set; }

        public virtual Household Household { get; set; }
        public int HouseholdId { get; set; }

        public virtual List<Transaction> Transactions { get; set; }

        public BankAccount()
        {
            DateCreated = DateTime.Now;
            Transactions = new List<Transaction>();
            Balance = 0;
        }
    }
}