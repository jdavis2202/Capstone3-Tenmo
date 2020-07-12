using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]    
    public class AccountsController : ControllerBase
    {
        private IAccountDAO accountDAO;
        private IUserDAO userDAO;

        public AccountsController(IAccountDAO passedAccountDAO, IUserDAO passedUserDAO)
        {
            accountDAO = passedAccountDAO;
            userDAO = passedUserDAO;
        }

        [HttpGet("balance")] // Gets the users account balance
        [Authorize]
        public ActionResult<decimal> GetBalance()
        {
            string authorizedName = User.Identity.Name;
            decimal userBalance = 0.00M;

            try
            {
                userBalance = accountDAO.GetBalance(authorizedName);

                if (userBalance < 0.00M)
                {
                    return NotFound("User balance is less than zero!");
                }
            }

            catch (Exception ex)
            {
                return NotFound($"Serverside Exception: {ex.Message}");
            }

            return Ok(userBalance); //something
        }

        [HttpGet("")] // returns ALL users in the system.
        [Authorize]
        public ActionResult<List<User>> ListAllAccounts()
        {
            List<User> users = new List<User>();

            try
            {
                users = userDAO.GetUsers();
                foreach (User user in users)
                {
                    user.PasswordHash = "";
                    user.Salt = "";
                }

                if (users.Count == 0)
                {
                    return NotFound("Serverside Exception: No users in database!");
                }
            }

            catch (Exception ex)
            {
                return NotFound($"Serverside Exception: {ex.Message}");
            }

            return Ok(users);
        }

        [HttpPut("send")]
        [Authorize]
        public ActionResult<string> SendMoney(Transfer transfer)
        {
            string status = "";

            try
            {
                status = accountDAO.SendTEBucks(transfer.sender, transfer.reciever, transfer.balance);

                if (String.IsNullOrEmpty(status))
                {
                    return BadRequest("Error while creating transfer.");
                }
            }
            catch (Exception ex)
            {
                return NotFound($"Serverside Exception: {ex.Message}");

            }

            return Ok(status);
        }

        [HttpGet("MyTransfers")]
        [Authorize]
        public ActionResult<List<Transfer>> GetMyTransfers()
        {
            List<Transfer> transfers = new List<Transfer>();
            string authorizedName = User.Identity.Name;
            try
            {
                transfers = accountDAO.GetMyTransfers(authorizedName);
            }
            catch (Exception ex)
            {
                return NotFound($"Serverside Exception: {ex.Message}");
            }

            return Ok(transfers);
        }


    }
}
