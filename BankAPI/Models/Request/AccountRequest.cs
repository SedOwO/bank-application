using BankAPI.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace BankAPI.Models.Request
{
    public class AccountRequest
    {
        public int CustomerId { get; set; }
        public int BankId { get; set; }
        public string? AccountNumber { get; set; }

        [Required]
        [EnumDataType(typeof(AccountTypes), ErrorMessage = "Invalid account type. Allowed values: Savings, Current.")]
        public AccountTypes AccountType { get; set; }
        public decimal Balance { get; set; }
    }
}
