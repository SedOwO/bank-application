using BankAPI.Models.DB;
using BankAPI.Models.Request;
using BankAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpGet]
        public async Task<ActionResult<Bank>> GetAllBanks()
        {
            var banks = await _bankService.GetAllBanksAsync();
            return Ok(banks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bank>> GetBankById(int id)
        {
            var bank = await _bankService.GetBankByIdAsync(id);

            if (bank == null)
            {
                return NotFound(new { Message = $"Bank with id {id} not found" });
            }

            return Ok(bank);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBank([FromBody] BankRequest bank)
        {
            try
            {
                var newBankId = await _bankService.CreateBankAsync(bank);

                var newBank = new Bank
                {
                    BankId = newBankId,
                    BankName = bank.BankName,
                    BranchName = bank.BranchName,
                    IfscCode = bank.IfscCode,
                    Address = bank.Address
                };

                return CreatedAtAction(nameof(GetBankById), new { id = newBankId }, newBank);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occured while creating the bank");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] BankRequest bank)
        {
            if (bank == null)
                return BadRequest("bank data is required");

            var updatedBank = await _bankService.UpdateBankAsync(id, bank);

            if (!updatedBank)
                return NotFound(new { message = $"Bank with ID {id} not found" });

            return Ok(new { message = $"Bank with ID {id} updated" });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            var deletedBank = await _bankService.DeleteBankAsync(id);

            if (!deletedBank)
                return NotFound(new { message = $"Bank with ID {id} not found!" });

            return Ok(new { message = $"Bank with ID {id} deleted successfully" });
        }
    }
}
