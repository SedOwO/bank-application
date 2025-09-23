using BankAPI.Models.DB;
using BankAPI.Models.Request;
using BankAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<Customer>> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound(new { Message = $"Customer with id {id} not found" });
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerRequest customer)
        {
            try
            {
                var newCustomerId = await _customerService.CreateCustomerAsync(customer);

                var newCustomer = new Customer
                {
                    CustomerId = newCustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Dob = customer.Dob,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address
                };

                return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomerId }, newCustomer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occured while creating the customer");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerRequest customer)
        {
            if (customer == null)
                return BadRequest("Customer data is required");

            var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customer);

            if (!updatedCustomer)
                return NotFound(new { message = $"Customer with ID {id} not found" });

            return Ok(new { message = $"Customer with ID {id} updated" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var deletedCustomer = await _customerService.DeleteCustomerAsync(id);

            if (!deletedCustomer)
                return NotFound(new { message = $"Customer with ID {id} not found!" });

            return Ok(new { message = $"Customer with ID {id} deleted successfully" });
        }
    }
}
