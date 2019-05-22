using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.BindingModel;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HouseholdBudgeter.Controllers
{
    [RoutePrefix("api/category-management")]
    public class CategoryManagementController : ApiController
    {
        private ApplicationDbContext DbContext;

        public CategoryManagementController()
        {
            DbContext = new ApplicationDbContext();
        }

        [Authorize]
        [HttpGet]
        [Route("get-by-id/{id:int}", Name = "CategoryById")]
        public IHttpActionResult CategoryById(int id)
        {
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == id);

            if(category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel(category);

            return Ok(viewModel);
        }

        [Authorize]
        [HttpPost]
        [Route("create-category/{id:int}")]
        public IHttpActionResult CreateCategory(int id, CategoryBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var houseHold = DbContext.Households.FirstOrDefault(p => p.Id == id);

            if(houseHold == null)
            {
                return NotFound();
            }

            var userId = User.Identity.GetUserId();

            if(userId != houseHold.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You are not allowed to create categories of this household.");
            }

            var category = new Category()
            {
                Name = model.Name,
                Description = model.Description,
                HouseholdId = houseHold.Id,
            };

            houseHold.Categories.Add(category);
            DbContext.SaveChanges();

            var url = Url.Link("CategoryById", new { Id = category.Id });

            var viewModel = new CategoryViewModel(category)
            {
                DateUpdated = null
            };

            return Created(url, viewModel);
        }

        [Authorize]
        [HttpPut]
        [Route("edit-category/{id:int}")]
        public IHttpActionResult EditCategory(int id, CategoryBindingModel model)
        {
            var userId = User.Identity.GetUserId();
            
            var category = DbContext.Categories.FirstOrDefault(p => p.Id == id);

            if(category == null)
            {
                return NotFound();
            }

            if(userId != category.Household.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You are not allowed to edit categories of this household.");
            }

            category.Name = model.Name;
            category.Description = model.Description;
            category.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();

            var viewModel = new CategoryViewModel(category);

            return Ok(viewModel);
        }
    }
}
