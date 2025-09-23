using System.ComponentModel.DataAnnotations;

namespace BankUI.Models
{
    public class Bank
    {
        public int BankId { get; set; }

        [Required(ErrorMessage = "Bank name is required.")]
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters.")]
        public string? BankName { get; set; }

        [Required(ErrorMessage = "Branch name is required.")]
        [StringLength(100, ErrorMessage = "Branch name cannot exceed 100 characters.")]
        public string? BranchName { get; set; }


        public string? IfscCode { get; set; }
        public string? Address { get; set; }
    }
}
