using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankUI.Pages.Banks
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Bank Bank { get; set; } = new();

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public void Onget() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var client = _clientFactory.CreateClient("BankApi");

            // Post new bank to api
            var response = await client.PostAsJsonAsync("bank", Bank);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Error creating bank.");
            return Page();
        }
    }
}
