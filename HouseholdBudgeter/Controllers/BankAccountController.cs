using HouseholdBudgeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HouseholdBudgeter.Controllers
{
    public class BankAccountController : ApiController
    {
        private ApplicationDbContext DbContext;

        public BankAccountController()
        {
            DbContext = new ApplicationDbContext();
        }


    }
}
