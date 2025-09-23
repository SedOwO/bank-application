using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;

namespace BankUI.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public int AccountId { get; set; }

        public Account? Account { get; set; }

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("BankApi");
            var response = await client.GetAsync($"account/{AccountId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ModelState.AddModelError(string.Empty, $"Acoount with ID {AccountId} was not found");
                return Page();
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error fetching account details.");
                return Page();
            }

            var account = await response.Content.ReadFromJsonAsync<Account>();

            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Customer not found");
                return Page();
            }

            Account = account;
            return Page();
        }


    }
}
