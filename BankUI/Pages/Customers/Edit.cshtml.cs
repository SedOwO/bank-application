using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankUI.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Customer Customer { get; set; } = new();

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("BankApi");
            var customer = await client.GetFromJsonAsync<Customer>($"customer/{id}");

            if (customer == null)
                return NotFound();

            Customer = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            // This used to throw Exception when the customer was not found
            // Fix was to GetAsync and check the status code prior to actually perform 
            // PutAsJsonAsync
            if (!ModelState.IsValid)
                return Page();

            var client = _clientFactory.CreateClient("BankApi");

            var response = await client.PutAsJsonAsync($"customer/{Customer.CustomerId}", Customer);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Error updating customer.");
            return Page();
        }
    }
}
