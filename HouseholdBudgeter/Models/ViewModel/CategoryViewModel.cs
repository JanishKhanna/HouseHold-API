using HouseholdBudgeter.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models.ViewModel
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public CategoryViewModel()
        {

        }

        public CategoryViewModel(Category category)
        {
            CategoryId = category.Id;
            Name = category.Name;
            Description = category.Description;
            DateCreated = category.DateCreated;
            DateUpdated = category.DateUpdated;
        }
    }    
}