using BankAPI.Messaging;
using BankAPI.Models.CustomExceptions;
using BankAPI.Models.DB;
using BankAPI.Models.Enum;
using BankAPI.Models.Request;
using BankAPI.Repositories.Implementations;
using BankAPI.Repositories.Interfaces;
using BankAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace BankAPI.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public AccountService(IAccountRepository accountRepository, IRabbitMqPublisher rabbitMqPublisher)
        {
            _accountRepository = accountRepository;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task<int> CreateAccountAsync(AccountRequest account)
        {

            if (!Enum.IsDefined(typeof(AccountTypes), account.AccountType))
                throw new ArgumentException("Invalid account type. Must be Savings or Current.");

            var accountId = await _accountRepository.CreateAccountAsync(account);

            if (accountId >= 0)
            {
                var message = $"Account created: {accountId} ({account.AccountNumber}), ID: {accountId}";
                await _rabbitMqPublisher.PublishMessage(message);
            }

            return accountId;
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            var result = await _accountRepository.DeleteAccountAsync(accountId);
            string message;

            if (result)
                message = $"Account deleted: {accountId}";
            else
                message = $"Account NOT deleted: {accountId}";

            await _rabbitMqPublisher.PublishMessage(message);


            return result;
        }

        public async Task<Account?> GetAccountByIdAsync(int accountId)
        {
            try
            {
                if (accountId <= 0)
                    return null;

                var account = await _accountRepository.GetAccountByIdAsync(accountId);

                if (account == null)
                    return null;

                if (!Enum.IsDefined(typeof(AccountTypes), account.AccountType))
                    throw new InvalidOperationException(
                        $"Invalid AccountType '{account.AccountType}' found for AccountId {accountId}."
                    );

                return account;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAccountsAsync();
        }

        public async Task<bool> UpdateAccountAsync(int accountId, AccountRequest account)
        {
            if (accountId <= 0)
            {
                throw new ServiceException("Invalid Customer ID");
            }

            var result = await _accountRepository.UpdateAccountAsync(accountId, account);

            var message = $"Account updated: {accountId} ({account.AccountNumber}), ID: {accountId}";
            await _rabbitMqPublisher.PublishMessage(message);

            return result;
        }
    }
}
