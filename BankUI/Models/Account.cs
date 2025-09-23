using BankUI.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace BankUI.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Bank ID is required.")]
        public int BankId { get; set; }


        public string? AccountNumber { get; set; }

        [Required(ErrorMessage = "Account Type is required.")]
        public AccountTypes AccountType { get; set; }

        public decimal Balance { get; set; }
    }
}
