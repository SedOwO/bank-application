using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankUI.Pages.Accounts
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Account Account { get; set; } = new();

        [TempData]
        public string? Message { get; set; }

        public List<Bank> Banks { get; set; } = new();

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("BankApi");
            var banks = await client.GetFromJsonAsync<List<Bank>>("bank");

            if (banks != null)
            {
                Banks = banks;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadBanksAsync();
                return Page();
            }
            var client = _clientFactory.CreateClient("BankApi");

            var customerResponse = await client.GetAsync($"customer/{Account.CustomerId}");
            
            if (customerResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ModelState.AddModelError("Account.CustomerId", $"Customer with ID {Account.CustomerId} does not exist.");
                await LoadBanksAsync();
                return Page();
            }

            if (!customerResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong while verifying the customer.");
                await LoadBanksAsync();
                return Page();
            }



            var response = await client.PostAsJsonAsync("account", Account);

            if (response.IsSuccessStatusCode)
            {
                var createdAccount = await response.Content.ReadFromJsonAsync<Account>();
                TempData["Message"] = $"Account created successfully! Account Number: {createdAccount.AccountId}";
                await LoadBanksAsync();
                return Page();
            }
            TempData["Message"] = "Account not created";
            ModelState.AddModelError(string.Empty, "Error creating account");
            return Page();
        }

        private async Task LoadBanksAsync()
        {
            var client = _clientFactory.CreateClient("BankApi");
            var banksResponse = await client.GetFromJsonAsync<List<Bank>>("bank");
            Banks = banksResponse ?? new List<Bank>();
        }

    }
}
