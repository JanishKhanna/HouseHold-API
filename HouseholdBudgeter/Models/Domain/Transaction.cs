﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Domain
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public decimal Amount { get; set; }
        public virtual Category Category { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public int BankAccountId { get; set; }

        public bool VoidTransaction { get; set; }

        public virtual ApplicationUser OwnerOfTransaction { get; set; }
        public string OwnerOfTransactionId { get; set; }

        public Transaction()
        {
            DateCreated = DateTime.Now;
            VoidTransaction = false;
        }
    }
}