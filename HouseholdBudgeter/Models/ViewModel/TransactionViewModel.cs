﻿using HouseholdBudgeter.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModel
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool VoidTransaction { get; set; }

        public TransactionViewModel()
        {

        }

        public TransactionViewModel(Transaction transaction)
        {
            TransactionId = transaction.Id;
            Title = transaction.Title;
            Description = transaction.Description;
            DateOfTransaction = transaction.DateOfTransaction;
            Amount = transaction.Amount;
            DateCreated = transaction.DateCreated;
            DateUpdated = transaction.DateUpdated;
            VoidTransaction = transaction.VoidTransaction;
        }
    }
}