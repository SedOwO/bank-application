using BankAPI.Models.Enum;

namespace BankAPI.Models.DB
{
    public class Account
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public int BankId { get; set; }
        public string? AccountNumber { get; set; }
        public AccountTypes AccountType { get; set; }
        public decimal Balance { get; set; }

    }
}
