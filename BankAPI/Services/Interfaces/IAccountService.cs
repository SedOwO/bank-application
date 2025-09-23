using BankAPI.Models.DB;
using BankAPI.Models.Request;

namespace BankAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Task<int> CreateAccountAsync(AccountRequest account);
        Task<Account?> GetAccountByIdAsync(int accountId);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<bool> UpdateAccountAsync(int accountId, AccountRequest account);
        Task<bool> DeleteAccountAsync(int accountId);
    }


}
