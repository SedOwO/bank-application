using BankUI.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankUI.Pages.Banks
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public List<Bank> Banks { get; set; } = new();

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("BankApi");
            var response = await client.GetFromJsonAsync<List<Bank>>("bank");
            if (response != null)
            {
                Banks = response.ToList();
            }
        }
    }
}
