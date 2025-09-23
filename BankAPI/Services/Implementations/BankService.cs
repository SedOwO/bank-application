using BankAPI.Models.DB;
using BankAPI.Models.CustomExceptions;
using BankAPI.Repositories.Interfaces;
using BankAPI.Services.Interfaces;
using BankAPI.Models.Request;
using BankAPI.Messaging;

namespace BankAPI.Services.Implementations
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public BankService(IBankRepository bankRepository, IRabbitMqPublisher rabbitMqPublisher)
        {
            _bankRepository = bankRepository;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task<int> CreateBankAsync(BankRequest bank)
        {
            if (string.IsNullOrWhiteSpace(bank.BankName))
                throw new ArgumentException("Bank Name required.");
            if (string.IsNullOrWhiteSpace(bank.BranchName))
                throw new ArgumentException("Branch Name required.");
            if (string.IsNullOrWhiteSpace(bank.IfscCode))
                throw new ArgumentException("IFSC Code required.");
            if (string.IsNullOrWhiteSpace(bank.Address))
                throw new ArgumentException("Address required.");

            var bankId = await _bankRepository.CreateBankAsync(bank);

            if (bankId >= 0)
            {
                var message = $"Bank created: {bank.BankName} ({bank.IfscCode}), ID: {bankId}";
                await _rabbitMqPublisher.PublishMessage(message);
            }

            return bankId;
        }

        public async Task<bool> DeleteBankAsync(int bankId)
        {
            var result = await _bankRepository.DeleteBankAsync(bankId);
            string message;

            if (result)
                message = $"Bank deleted: {bankId}";
            else
                message = $"Bank NOT deleted: {bankId}";

            await _rabbitMqPublisher.PublishMessage(message);
            return result;
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync()
        {
            return await _bankRepository.GetAllBanksAsync();
        }

        public async Task<Bank?> GetBankByIdAsync(int bankId)
        {
            try
            {
                if (bankId <= 0)
                    return null;

                var bank = await _bankRepository.GetBankByIdAsync(bankId);

                if (bank == null)
                    return null;

                bank.BankName = bank.BankName.Trim();

                return bank;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateBankAsync(int bankId, BankRequest bank)
        {
            if (bankId <= 0)
            {
                throw new ServiceException("Invalid Bank ID");
            }

            var result = await _bankRepository.UpdateBankAsync(bankId, bank);

            var message = $"Bank updated: {bank.BankName} ({bank.IfscCode}), ID: {bankId}";
            await _rabbitMqPublisher.PublishMessage(message);

            return result;

        }
    }
}
