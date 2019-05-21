using HouseholdBudgeter.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModel
{
    public class HouseholdViewModel
    {
        public int HouseholdId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public HouseholdViewModel()
        {

        }

        public HouseholdViewModel(Household household)
        {
            HouseholdId = household.Id;
            Name = household.Name;
            Description = household.Description;
            DateCreated = household.DateCreated;
            DateUpdated = household.DateUpdated;
        }
    }
}