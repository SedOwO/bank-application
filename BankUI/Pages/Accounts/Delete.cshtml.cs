using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankUI.Pages.Accounts
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Account Account { get; set; } = new();

        public DeleteModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("BankApi");
            var account = await client.GetFromJsonAsync<Account>($"account/{id}");

            if (account == null)
            {
                return NotFound();
            }
            Account = account;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var client = _clientFactory.CreateClient("BankApi");
            var response = await client.DeleteAsync($"account/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Error deleting account");
            return Page();

        }
    }
}
