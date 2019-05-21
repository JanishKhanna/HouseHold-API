using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.Domain
{
    public class Household
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        [InverseProperty(nameof(ApplicationUser.OwnnedHouses))]
        public virtual ApplicationUser OwnerOfHouse { get; set; }
        public string OwnerOfHouseId { get; set; }

        [InverseProperty(nameof(ApplicationUser.InvitedToHouses))]
        public virtual List<ApplicationUser> InvitedUsers { get; set; }

        [InverseProperty(nameof(ApplicationUser.JoinedToHouses))]
        public virtual List<ApplicationUser> JoinedUsers { get; set; }

        public virtual List<Category> Categories { get; set; }

        public Household()
        {
            Categories = new List<Category>();
            DateCreated = DateTime.Now;
            InvitedUsers = new List<ApplicationUser>();
            JoinedUsers = new List<ApplicationUser>();
        }
    }
}