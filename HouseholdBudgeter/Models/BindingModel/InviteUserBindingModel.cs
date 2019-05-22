using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.BindingModel
{
    public class InviteUserBindingModel
    {
        [Required]
        public string Email { get; set; }
    }
}