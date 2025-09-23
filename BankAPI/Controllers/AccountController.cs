using BankAPI.Models.DB;
using BankAPI.Models.Request;
using BankAPI.Services.Implementations;
using BankAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;   
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountRequest account)
        {
            try
            {
                var newAccountId = await _accountService.CreateAccountAsync(account);

                var newAccount = new Account
                {
                    AccountId = newAccountId,
                    BankId = account.BankId,
                    CustomerId = account.CustomerId,
                    AccountNumber = account.AccountNumber,
                    AccountType = account.AccountType,
                    Balance = account.Balance
                };

                return CreatedAtAction(nameof(GetAccountById), new { id = newAccountId }, newAccount);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occured while creating the Account");
            }
        }

        [HttpGet]
        public async Task<ActionResult<Account>> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);

            if (account == null)
            {
                return NotFound(new { Message = $"Account with id {id} not found" });
            }

            return Ok(account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountRequest account)
        {
            if (account == null)
                return BadRequest("Account data is required");

            var updatedAccount = await _accountService.UpdateAccountAsync(id, account);

            if (!updatedAccount)
                return NotFound(new { message = $"Account with ID {id} not found" });

            return Ok(new { message = $"Account with ID {id} updated" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var deletedAccount = await _accountService.DeleteAccountAsync(id);
            Console.WriteLine("Deleted account: " + deletedAccount);

            if (!deletedAccount)
                return NotFound(new { message = $"Account with ID {id} not found!" });

            return Ok(new { message = $"Account with ID {id} deleted successfully" });
        }

    }
}
