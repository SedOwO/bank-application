namespace BankAPI.Models.Request
{
    public class BankRequest
    {
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? IfscCode { get; set; }
        public string? Address { get; set; }
    }
}
