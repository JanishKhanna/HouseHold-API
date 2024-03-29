﻿using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.BindingModel;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HouseholdBudgeter.Controllers
{
    [RoutePrefix("api/household-management")]
    public class HouseholdManagementController : ApiController
    {
        private ApplicationDbContext DbContext;

        public HouseholdManagementController()
        {
            DbContext = new ApplicationDbContext();
        }

        [Authorize]
        [HttpGet]
        [Route("get-all-households")]
        public IHttpActionResult AllHouseholds()
        {
            var userId = User.Identity.GetUserId();

            var listOfHouseholds = DbContext.Households
                .Where(p => p.OwnerOfHouseId == userId ||
                p.InvitedUsers.Any(t => t.Id == userId) ||
                p.JoinedUsers.Any(m => m.Id == userId)).ToList();

            var viewModel = listOfHouseholds
                .Select(p => new HouseholdViewModel(p)
                {
                    IsOwner = p.OwnerOfHouseId == userId,
                    NumberOfUsers = p.JoinedUsers.Count() + 1,
                });

            return Ok(viewModel);
        }

        [Authorize]
        [HttpGet]
        [Route("by-id/{id:int}", Name = "HouseById")]
        public IHttpActionResult HouseById(int id)
        {
            var userId = User.Identity.GetUserId();

            var myHouseHold = DbContext.Households.FirstOrDefault(p => p.Id == id);

            if (myHouseHold == null)
            {
                return NotFound();
            }

            var viewModel = new HouseholdViewModel(myHouseHold)
            {
                IsOwner = myHouseHold.OwnerOfHouse.Id == userId
            };

            return Ok(viewModel);
        }

        [Authorize]
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateHouseHold(HouseholdBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var owner = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            var houseHold = new Household()
            {
                Name = model.Name,
                Description = model.Description,
                DateCreated = DateTime.Now,
                OwnerOfHouse = owner,
                OwnerOfHouseId = owner.Id
            };

            DbContext.Households.Add(houseHold);
            DbContext.SaveChanges();

            var url = Url.Link("HouseById", new { Id = houseHold.Id });

            var viewModel = new HouseholdViewModel(houseHold)
            {
                DateUpdated = null
            };

            return Created(url, viewModel);
        }

        [Authorize]
        [HttpPut]
        [Route("edit/{id:int}")]
        public IHttpActionResult Edit(int id, HouseholdBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var houseHold = DbContext.Households.FirstOrDefault(p => p.Id == id);

            if (houseHold == null)
            {
                return NotFound();
            }

            if (userId != houseHold.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You are not allowed to edit this household.");
            }

            houseHold.Name = model.Name;
            houseHold.Description = model.Description;
            houseHold.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();

            var viewModel = new HouseholdViewModel(houseHold);

            return Ok(viewModel);
        }

        [Authorize]
        [HttpDelete]
        [Route("delete/{id:int}")]
        public IHttpActionResult DeleteHouseHold(int id)
        {
            var userId = User.Identity.GetUserId();
            var houseHold = DbContext.Households.FirstOrDefault(p => p.Id == id);

            if (houseHold == null)
            {
                return NotFound();
            }

            if (userId != houseHold.OwnerOfHouseId)
            {
                if (houseHold.JoinedUsers.Any(p => p.Id == userId))
                {
                    var currentUser = DbContext.Users.First(p => p.Id == userId);

                    houseHold.JoinedUsers.Remove(currentUser);
                    DbContext.SaveChanges();

                    return Ok($"You have left the household with id:{houseHold.Id}");
                }

                return BadRequest("Sorry, You are not allowed to delete this household.");
            }

            DbContext.Households.Remove(houseHold);
            DbContext.SaveChanges();

            return Ok($"Your Household with Id:{id} has been Deleted.");
        }

        [Authorize]
        [HttpPost]
        [Route("invite/{id:int}")]
        public IHttpActionResult InviteUsers(int id, InviteUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var houseHold = DbContext.Households.FirstOrDefault(p => p.Id == id);

            if (houseHold == null)
            {
                return NotFound();
            }

            if (userId != houseHold.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You cannot send invite to users.");
            }

            var inviteUser = DbContext.Users.FirstOrDefault(p => p.Email == model.Email);

            if (inviteUser == null)
            {
                ModelState.AddModelError("", "User doesn't not exist");
                return BadRequest(ModelState);
            }

            if (inviteUser.Id == houseHold.OwnerOfHouseId)
            {
                return BadRequest("You cannot invite yourself to the household.");
            }

            houseHold.InvitedUsers.Add(inviteUser);
            DbContext.SaveChanges();

            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            userManager.SendEmail(inviteUser.Id, "Invitation to the Household", $"You are invited to the Household with id:{houseHold.Id}.");

            return Ok("User Invited to the Household");
        }

        [Authorize]
        [HttpGet]
        [Route("get-invites")]
        public IHttpActionResult GetInvites()
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            var viewModel = user.InvitedToHouses
                .Select(p => new InviteViewModel
                {
                    HouseholdId = p.Id,
                    Name = p.Name,
                    OwnerName = p.OwnerOfHouse.UserName
                }).ToList();

            return Ok(viewModel);
        }

        [Authorize]
        [HttpPost]
        [Route("join/{id:int}")]
        public IHttpActionResult Join(int id)
        {
            var userId = User.Identity.GetUserId();
            var houseHold = DbContext.Households.FirstOrDefault(p => p.Id == id);

            if (houseHold == null)
            {
                return NotFound();
            }

            if (userId == houseHold.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You cannot join this household.");
            }

            if (!houseHold.InvitedUsers.Any(p => p.Id == userId))
            {
                return BadRequest("You were not invited to this household");
            }

            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            if (currentUser == null)
            {
                return NotFound();
            }

            houseHold.InvitedUsers.Remove(currentUser);
            houseHold.JoinedUsers.Add(currentUser);
            DbContext.SaveChanges();

            return Ok($"You Joined the household with id:{houseHold.Id}");
        }

        [Authorize]
        [HttpGet]
        [Route("users-joined-to-household/{id:int}")]
        public IHttpActionResult ListOfJoinedUsers(int id)
        {
            var userId = User.Identity.GetUserId();
            var houseHold = DbContext.Households.FirstOrDefault(p => p.Id == id);

            if (houseHold == null)
            {
                return NotFound();
            }

            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            if (currentUser != houseHold.OwnerOfHouse && !houseHold.JoinedUsers.Contains(currentUser))
            {
                return Unauthorized();
            }

            var viewModel = houseHold.JoinedUsers.Select(p => new UserViewModel
            {
                Email = p.Email,
            });

            return Ok(viewModel);
        }

        //[Authorize]
        //[HttpGet]
        //[Route("details/{id:int}")]
        //public IHttpActionResult Details(int id)
        //{
        //    var userId = User.Identity.GetUserId();
        //    var household = DbContext.Households.FirstOrDefault(p => p.Id == id);
        //    var bankAccounts = household.BankAccounts;
        //}
    }
}