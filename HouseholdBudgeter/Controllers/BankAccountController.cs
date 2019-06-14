using HouseholdBudgeter.Models;
using HouseholdBudgeter.Models.BindingModel;
using HouseholdBudgeter.Models.Domain;
using HouseholdBudgeter.Models.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HouseholdBudgeter.Controllers
{
    [Authorize]
    [RoutePrefix("api/bank-account")]
    public class BankAccountController : ApiController
    {
        private ApplicationDbContext DbContext;

        public BankAccountController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpGet]
        [Route("account-by-id/{id:int}", Name = "AccountById")]
        public IHttpActionResult AccountById(int id)
        {
            var userId = User.Identity.GetUserId();

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            var viewModel = new BankAccountViewModel(bankAccount)
            {
                IsOwner = bankAccount.Household.OwnerOfHouse.Id == userId
            };

            return Ok(viewModel);
        }

        [HttpPost]
        [Route("create-bankaccount/{id:int}")]
        public IHttpActionResult CreateBankAccount(int id, BankAccountBindingModel model)
        {
            if(model == null)
            {
                ModelState.AddModelError(nameof(model), "Invalid form data");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var houseHold = DbContext.Households.FirstOrDefault(p => p.Id == id);

            if (houseHold == null)
            {
                return NotFound();
            }

            var userId = User.Identity.GetUserId();

            if (userId != houseHold.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You are not allowed to create a Bank Account of this household.");
            }

            var bankAccount = new BankAccount()
            {
                Name = model.Name,
                Description = model.Description,
            };

            houseHold.BankAccounts.Add(bankAccount);
            DbContext.SaveChanges();

            var url = Url.Link("AccountById", new { Id = bankAccount.Id });

            var viewModel = new BankAccountViewModel(bankAccount)
            {
                DateUpdated = null,
                IsOwner = houseHold.OwnerOfHouse.Id == userId
            };

            return Created(url, viewModel);
        }

        [HttpPut]
        [Route("edit-account/{id:int}")]
        public IHttpActionResult EditAccount(int id, BankAccountBindingModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError(nameof(model), "Invalid form data");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            if (userId != bankAccount.Household.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You are not allowed to edit this Bank Account.");
            }

            bankAccount.Name = model.Name;
            bankAccount.Description = model.Description;
            bankAccount.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();

            var viewModel = new BankAccountViewModel(bankAccount)
            {
                IsOwner = bankAccount.Household.OwnerOfHouse.Id == userId
            };

            return Ok(viewModel);
        }

        [HttpDelete]
        [Route("delete-account/{id:int}")]
        public IHttpActionResult DeleteAccount(int id)
        {
            var userId = User.Identity.GetUserId();

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            if (userId != bankAccount.Household.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You are not allowed to delete Bank Accounts of this household.");
            }

            DbContext.BankAccounts.Remove(bankAccount);
            DbContext.SaveChanges();

            return Ok($"You have deleted the Bank Account with id:{bankAccount.Id}");
        }

        [HttpGet]
        [Route("list-of-accounts/{id:int}")]
        public IHttpActionResult ListOfBankAccounts(int id)
        {
            var userId = User.Identity.GetUserId();
            var houseHold = DbContext.Households
                .Include(p => p.BankAccounts)
                .FirstOrDefault(p => p.Id == id);

            if (houseHold == null)
            {
                return NotFound();
            }

            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            if (currentUser != houseHold.OwnerOfHouse && !houseHold.JoinedUsers.Contains(currentUser))
            {
                return Unauthorized();
            }

            var viewModel = houseHold.BankAccounts.Select(p => new BankAccountViewModel(p)
            {
                IsOwner = houseHold.OwnerOfHouse.Id == userId
            }).ToList();

            return Ok(viewModel);            
        }

        [HttpPut]
        [Route("update-balance/{id:int}")]
        public IHttpActionResult UpdateBalance(int id)
        {
            var userId = User.Identity.GetUserId();
            var bankAccount = DbContext.BankAccounts
                .Include(p => p.Transactions)
                .FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            if (userId != bankAccount.Household.OwnerOfHouseId)
            {
                return BadRequest("Sorry, You are not allowed to Update the Balance.");
            }

            var total = bankAccount.Transactions
                .Where(p => p.VoidTransaction == false)
                .Select(p => p.Amount)
                .Sum();

            bankAccount.Balance = total;

            DbContext.SaveChanges();

            return Ok(bankAccount.Balance);
        }
    }
}