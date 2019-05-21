using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using HouseholdBudgeter.Models.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace HouseholdBudgeter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty(nameof(Household.OwnerOfHouse))]
        public virtual List<Household> OwnnedHouses { get; set; }

        [InverseProperty(nameof(Household.InvitedUsers))]
        public virtual List<Household> InvitedToHouses { get; set; }

        [InverseProperty(nameof(Household.JoinedUsers))]
        public virtual List<Household> JoinedToHouses { get; set; }

        public ApplicationUser()
        {
            OwnnedHouses = new List<Household>();
            InvitedToHouses = new List<Household>();
            JoinedToHouses = new List<Household>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Household> Households { get; set; }
        public DbSet<Category> Categories { get; set; } 
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}