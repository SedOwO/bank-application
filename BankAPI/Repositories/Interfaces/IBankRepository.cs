using BankAPI.Models.DB;
using BankAPI.Models.Request;

namespace BankAPI.Repositories.Interfaces
{
    public interface IBankRepository
    {
        Task<Bank> GetBankByIdAsync(int bankId);
        Task<IEnumerable<Bank>> GetAllBanksAsync();
        Task<int> CreateBankAsync(BankRequest bank);
        Task<bool> UpdateBankAsync(int bankId, BankRequest bank);
        Task<bool> DeleteBankAsync(int bankId);

    }
}
