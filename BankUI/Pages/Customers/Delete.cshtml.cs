using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankUI.Pages.Customers
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Customer Customer { get; set; } = new();

        public DeleteModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("BankApi");
            var customer = await client.GetFromJsonAsync<Customer>($"customer/{id}");

            if (customer == null)
            {
                return NotFound();
            }

            Customer = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var client = _clientFactory.CreateClient("BankApi");

            var response = await client.DeleteAsync($"customer/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Error deleting customer.");

            return Page();

        }
    }
}
