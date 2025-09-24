using BankAPI.Models.DB;
using BankAPI.Models.Enum;
using BankAPI.Models.Request;
using BankAPI.Repositories.Interfaces;
using Npgsql;

namespace BankAPI.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<int> CreateAccountAsync(AccountRequest account)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.CreateAccount(@customerid, @bankid, @accounttype, @balance);", conn);

                cmd.Parameters.AddWithValue("customerid", account.CustomerId);
                cmd.Parameters.AddWithValue("bankid", account.BankId);
                cmd.Parameters.AddWithValue("accounttype", account.AccountType.ToString());
                cmd.Parameters.AddWithValue("balance", account.Balance);

                var result = await cmd.ExecuteScalarAsync();

                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.DeleteAccount(@accountid);", conn);

                cmd.Parameters.AddWithValue("accountid", accountId);

                var result = await cmd.ExecuteScalarAsync();

                return result != null && (bool)result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM public.GetAccountByID(@accountid)", conn);

                cmd.Parameters.AddWithValue("accountid", accountId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Account
                    {
                        AccountId = reader.GetInt32(reader.GetOrdinal("accountid")),
                        CustomerId = reader.GetInt32(reader.GetOrdinal("customerid")),
                        BankId = reader.GetInt32(reader.GetOrdinal("customerid")),
                        AccountNumber = reader.GetString(reader.GetOrdinal("accountnumber")),
                        AccountType = Enum.Parse<AccountTypes>(reader.GetString(reader.GetOrdinal("accounttype"))),
                        Balance = reader.GetDecimal(reader.GetOrdinal("balance"))
                    };
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            try
            {
                var accounts = new List<Account>();

                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM public.GetAllAccounts()", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    accounts.Add(new Account
                    {
                        AccountId = reader.GetInt32(reader.GetOrdinal("accountid")),
                        CustomerId = reader.GetInt32(reader.GetOrdinal("customerid")),
                        BankId = reader.GetInt32(reader.GetOrdinal("customerid")),
                        AccountNumber = reader.GetString(reader.GetOrdinal("accountnumber")),
                        AccountType = Enum.Parse<AccountTypes>(reader.GetString(reader.GetOrdinal("accounttype"))),
                        Balance = reader.GetDecimal(reader.GetOrdinal("balance"))
                    });
                }
                return accounts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAccountAsync(int accountId, AccountRequest account)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.UpdateAccount(@accountid ,@accounttype, @balance);", conn);

                cmd.Parameters.AddWithValue("accountid", accountId);
                cmd.Parameters.AddWithValue("accounttype", account.AccountType.ToString());
                cmd.Parameters.AddWithValue("balance", account.Balance);

                var result = await cmd.ExecuteScalarAsync();

                return result != null && (bool)result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
