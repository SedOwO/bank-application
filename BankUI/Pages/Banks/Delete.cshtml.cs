using BankUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankUI.Pages.Banks
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Bank Bank { get; set; } = new();

        public DeleteModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("BankApi");
            var bank = await client.GetFromJsonAsync<Bank>($"bank?id={id}");

            if (bank == null)
            {
                return NotFound();
            }

            Bank = bank;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var client = _clientFactory.CreateClient("BankApi");

            var response = await client.DeleteAsync($"bank?id={id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Error deleting bank.");
            return Page();
        }
    }
}
