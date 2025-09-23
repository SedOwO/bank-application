using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankUI.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;


        [BindProperty]
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("BankApi");

            var response = await client.GetAsync($"customer/{CustomerId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Customer with ID {CustomerId} was not found.");
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error fetching customer details.");
                return Page();
            }

            var customer = await response.Content.ReadFromJsonAsync<Customer>();

            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Customer not found");
                return Page();
            }

            Customer = customer;
            return Page();
        }
    }
}
