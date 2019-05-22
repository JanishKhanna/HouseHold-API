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

        public List<CategoryViewModel> Categories { get; set; }
        
        public List<UserViewModel> InvitedUsers { get; set; }

        public List<UserViewModel> JoinedUsers { get; set; }

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
            Categories = household.Categories.Select(p => new CategoryViewModel(p)).ToList();
            InvitedUsers = household.InvitedUsers.Select(p => new UserViewModel(p)).ToList();
            JoinedUsers = household.JoinedUsers.Select(p => new UserViewModel(p)).ToList();
        }
    }
}