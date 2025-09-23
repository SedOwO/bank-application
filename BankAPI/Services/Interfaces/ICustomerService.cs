using BankAPI.Models.DB;
using BankAPI.Models.Request;

namespace BankAPI.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<int> CreateCustomerAsync(CustomerRequest customer);
        Task<bool> UpdateCustomerAsync(int customerId, CustomerRequest customer);
        Task<bool> DeleteCustomerAsync(int customerId);
    }
}
