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
    [Authorize]
    [RoutePrefix("api/transaction")]
    public class TransactionController : ApiController
    {
        private ApplicationDbContext DbContext;

        public TransactionController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpGet]
        [Route("transaction-by-id/{id:int}", Name = "TransactionById")]
        public IHttpActionResult TransactionById(int id)
        {
            var userId = User.Identity.GetUserId();
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            var viewModel = new TransactionViewModel(transaction)
            {
                IsOwner = transaction.BankAccount.Household.OwnerOfHouseId == userId
            };

            return Ok(viewModel);
        }

        [HttpPost]
        [Route("create-transaction/{accountId:int}")]
        public IHttpActionResult CreateTransaction(int accountId, TransactionBindingModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError(nameof(model), "Invalid form data");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == accountId);

            if (bankAccount == null)
            {
                return NotFound();
            }

            var category = DbContext.Categories.FirstOrDefault(p => p.Id == model.CategoryId);

            if (category == null)
            {
                return NotFound();
            }

            if (category.HouseholdId != bankAccount.HouseholdId)
            {
                return NotFound();
            }

            var userId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            if (currentUser.Id != bankAccount.Household.OwnerOfHouseId && !bankAccount.Household.JoinedUsers.Contains(currentUser))
            {
                return Unauthorized();
            }

            var transaction = new Transaction()
            {
                Title = model.Title,
                Description = model.Description,
                DateOfTransaction = model.DateOfTransaction,
                Amount = model.Amount,
                DateCreated = DateTime.Now,
                CategoryId = category.Id,
                OwnerOfTransaction = currentUser
            };

            bankAccount.Balance += transaction.Amount;
            bankAccount.Transactions.Add(transaction);
            DbContext.SaveChanges();

            var url = Url.Link("TransactionById", new { Id = transaction.Id });

            var viewModel = new TransactionViewModel(transaction)
            {
                IsOwner = bankAccount.Household.OwnerOfHouseId == userId || transaction.OwnerOfTransactionId == userId
            };

            return Created(url, viewModel);
        }

        [HttpPut]
        [Route("edit-transaction/{id:int}")]
        public IHttpActionResult EditTransaction(int id, EditTransactionBindingModel model)
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
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.BankAccount.Household.OwnerOfHouseId != userId && transaction.OwnerOfTransactionId != userId)
            {
                return Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                transaction.Title = model.Title;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                transaction.Description = model.Description;
            }

            if (model.DateOfTransaction != default)
            {
                transaction.DateOfTransaction = model.DateOfTransaction;
            }

            if (model.Amount != default)
            {
                transaction.BankAccount.Balance -= transaction.Amount;
                transaction.Amount = model.Amount;
                transaction.BankAccount.Balance += transaction.Amount;
            }

            transaction.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();

            var viewModel = new TransactionViewModel(transaction)
            {
                IsOwner = transaction.BankAccount.Household.OwnerOfHouseId == userId || transaction.OwnerOfTransactionId == userId
            };

            return Ok(viewModel);
        }

        [HttpDelete]
        [Route("delete-transaction/{id:int}")]
        public IHttpActionResult DeleteTransaction(int id)
        {
            var userId = User.Identity.GetUserId();
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.BankAccount.Household.OwnerOfHouseId != userId && transaction.OwnerOfTransactionId != userId)
            {
                return Unauthorized();
            }

            if (!transaction.VoidTransaction)
            {
                transaction.BankAccount.Balance -= transaction.Amount;
            }

            DbContext.Transactions.Remove(transaction);
            DbContext.SaveChanges();

            return Ok($"You have deleted the transaction with id:{transaction.Id}");
        }

        [HttpPut]
        [Route("void-transaction/{id:int}")]
        public IHttpActionResult Void(int id)
        {
            var userId = User.Identity.GetUserId();
            var transaction = DbContext.Transactions.FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.BankAccount.Household.OwnerOfHouseId != userId && transaction.OwnerOfTransactionId != userId)
            {
                return Unauthorized();
            }

            transaction.VoidTransaction = true;
            transaction.BankAccount.Balance -= transaction.Amount;

            DbContext.SaveChanges();

            var viewModel = new TransactionViewModel(transaction)
            {
                IsOwner = transaction.BankAccount.Household.OwnerOfHouseId == userId || transaction.OwnerOfTransactionId == userId
            };

            return Ok(viewModel);
        }

        [HttpGet]
        [Route("list-of-transactions/{accountId:int}")]
        public IHttpActionResult ListOfTransaction(int accountId)
        {
            var userId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.First(p => p.Id == userId);
            var bankAccount = DbContext.BankAccounts.FirstOrDefault(p => p.Id == accountId);

            if (bankAccount == null)
            {
                return NotFound();
            }

            if (bankAccount.Household.OwnerOfHouseId != userId && !bankAccount.Household.JoinedUsers.Contains(currentUser))
            {
                return Unauthorized();
            }

            var viewModel = bankAccount.Transactions.Select(p => new TransactionViewModel(p)
            {
                IsOwner = bankAccount.Household.OwnerOfHouseId == userId || p.OwnerOfTransactionId == userId
            }).ToList();

            return Ok(viewModel);
        }
    }
}
