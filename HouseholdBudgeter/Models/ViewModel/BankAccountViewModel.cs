using HouseholdBudgeter.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModel
{
    public class BankAccountViewModel
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public decimal Balance { get; set; }
        public int HouseholdId { get; set; }
        public bool IsOwner { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }

        public BankAccountViewModel()
        {

        }

        public BankAccountViewModel(BankAccount bankAccount)
        {
            HouseholdId = bankAccount.HouseholdId;
            AccountId = bankAccount.Id;
            Name = bankAccount.Name;
            Description = bankAccount.Description;
            DateCreated = bankAccount.DateCreated;
            DateUpdated = bankAccount.DateUpdated;
            Balance = bankAccount.Balance;
            Transactions = bankAccount.Transactions.Select(p => new TransactionViewModel(p)).ToList();
        }
    }
}