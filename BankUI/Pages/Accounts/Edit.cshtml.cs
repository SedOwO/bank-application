using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace BankUI.Pages.Accounts
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Account Account { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        public List<Bank> Banks { get; set; } = new();

        public SelectList BankOptions { get; set; } = default!;


        public EditModel(IHttpClientFactory clientFactory)
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

            await LoadBanksAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadBanksAsync();
                return Page();
            }
            await LoadBanksAsync();

            var client = _clientFactory.CreateClient("BankApi");

            var response = await client.PutAsJsonAsync($"account/{Account.AccountId}", Account);

            // restriction: customerId cannot be changed
            // restriction: account number cannot be changed

            if (response.IsSuccessStatusCode)
            {
                var createdAccount = await response.Content.ReadFromJsonAsync<Account>();
                TempData["Message"] = $"Account edited successfully! Account Number: {createdAccount.AccountId}";
                await LoadBanksAsync();
                return Page();
            }

            TempData["Message"] = "Account could not be edited";
            ModelState.AddModelError(string.Empty, "Error editing account");
            return Page();
        }

        private async Task LoadBanksAsync()
        {
            var client = _clientFactory.CreateClient("BankApi");
            var banksResponse = await client.GetFromJsonAsync<List<Bank>>("bank");
            Banks = banksResponse ?? new List<Bank>();

            BankOptions = new SelectList(Banks, "BankId", "BankName", Account?.BankId);
        }
    }
}
