namespace BankAPI.Models.Response
{
    public class BankResponse
    {
        public int BankId { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public string? IfscCode { get; set; }
        public string? Address { get; set; }
    }
}
