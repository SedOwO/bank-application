using BankAPI.Models.DB;
using BankAPI.Models.Request;
using BankAPI.Repositories.Interfaces;
using Npgsql;
using System.Security.AccessControl;

namespace BankAPI.Repositories.Implementations
{
    public class BankRepository : IBankRepository
    {
        private readonly string _connectionString;
        public BankRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<int> CreateBankAsync(BankRequest bank)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.CreateBank(@bankname, @branchname, @ifsccode, @address);", conn);

                cmd.Parameters.AddWithValue("bankname", bank.BankName);
                cmd.Parameters.AddWithValue("branchname", bank.BranchName);
                cmd.Parameters.AddWithValue("ifsccode", bank.IfscCode);
                cmd.Parameters.AddWithValue("address", bank.Address);

                var result = await cmd.ExecuteScalarAsync();

                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteBankAsync(int bankId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.DeleteBank(@p_bankid);", conn);

                cmd.Parameters.AddWithValue("p_bankid", bankId);

                var result = await cmd.ExecuteScalarAsync();

                return result != null && (bool)result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync()
        {
            try
            {
                var banks = new List<Bank>();

                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM public.GetAllBanks()", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    banks.Add(new Bank
                    {
                        BankId = reader.GetInt32(reader.GetOrdinal("bankid")),
                        BankName = reader.GetString(reader.GetOrdinal("bankname")),
                        BranchName = reader.GetString(reader.GetOrdinal("branchname")),
                        IfscCode = reader.GetString(reader.GetOrdinal("ifsccode")),
                        Address = reader.GetString(reader.GetOrdinal("address"))
                    });
                }
                return banks;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Get bank by id
        // <Bank?> because the funciton is nullable(may return null value)
        public async Task<Bank?> GetBankByIdAsync(int bankId)
        {
			try
			{
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM public.GetBankByID(@p_bankid)", conn);

                cmd.Parameters.AddWithValue("p_bankid", bankId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Bank
                    {
                        BankId = reader.GetInt32(reader.GetOrdinal("bankid")),
                        BankName = reader.GetString(reader.GetOrdinal("bankname")),
                        BranchName = reader.GetString(reader.GetOrdinal("branchname")),
                        IfscCode = reader.GetString(reader.GetOrdinal("ifsccode")),
                        Address = reader.GetString(reader.GetOrdinal("address"))
                    };
                }
                return null;
			}
			catch (Exception)
			{
				throw;
			}
        }

        public async Task<bool> UpdateBankAsync(int bankId, BankRequest bank)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.UpdateBank(@p_bankid, @p_bankname, @p_branchname, @p_ifsccode, @p_address);", conn);

                cmd.Parameters.AddWithValue("p_bankid", bankId);
                cmd.Parameters.AddWithValue("p_bankname", bank.BankName);
                cmd.Parameters.AddWithValue("p_branchname", bank.BranchName);
                cmd.Parameters.AddWithValue("p_ifsccode", bank.IfscCode);
                cmd.Parameters.AddWithValue("p_address", bank.Address);

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
