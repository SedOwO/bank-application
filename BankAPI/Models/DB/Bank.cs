namespace BankAPI.Models.DB
{
    public class Bank
    {
        public int BankId { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? IfscCode { get; set; }
        public string? Address { get; set; }
    }
}
