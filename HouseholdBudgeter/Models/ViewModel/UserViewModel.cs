using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModel
{
    public class UserViewModel
    {
        public string Email { get; set; }

        public UserViewModel()
        {

        }

        public UserViewModel(ApplicationUser user)
        {
            Email = user.Email;
        }
    }
}