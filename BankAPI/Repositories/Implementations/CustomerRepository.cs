using BankAPI.Models.DB;
using BankAPI.Models.Request;
using BankAPI.Repositories.Interfaces;
using Npgsql;

namespace BankAPI.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<int> CreateCustomerAsync(CustomerRequest customer)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.CreateCustomer(@firstname, @lastname, @dob, @email, @phone, @address);", conn);

                cmd.Parameters.AddWithValue("firstname", customer.FirstName);
                cmd.Parameters.AddWithValue("lastname", customer.LastName);
                cmd.Parameters.AddWithValue("dob", NpgsqlTypes.NpgsqlDbType.Date, customer.Dob.Date);
                cmd.Parameters.AddWithValue("email", customer.Email);
                cmd.Parameters.AddWithValue("phone", customer.Phone);
                cmd.Parameters.AddWithValue("address", customer.Address);

                var result = await cmd.ExecuteScalarAsync();

                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.DeleteCustomer(@customerid);", conn);

                cmd.Parameters.AddWithValue("customerid", customerId);

                var result = await cmd.ExecuteScalarAsync();

                return result != null && (bool)result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            try
            {
                var customers = new List<Customer>();

                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM public.GetAllCustomers()", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    customers.Add(new Customer
                    {
                        CustomerId = reader.GetInt32(reader.GetOrdinal("customerid")),
                        FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                        LastName = reader.GetString(reader.GetOrdinal("lastname")),
                        Dob = reader.GetDateTime(reader.GetOrdinal("dob")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Phone = reader.GetString(reader.GetOrdinal("phone")),
                        Address = reader.GetString(reader.GetOrdinal("address"))
                    });
                }
                return customers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT * FROM public.GetCustomerByID(@customerid)", conn);

                cmd.Parameters.AddWithValue("customerid", customerId);

                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Customer
                    {
                        CustomerId = reader.GetInt32(reader.GetOrdinal("customerid")),
                        FirstName = reader.GetString(reader.GetOrdinal("firstname")),
                        LastName = reader.GetString(reader.GetOrdinal("lastname")),
                        Dob = reader.GetDateTime(reader.GetOrdinal("dob")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Phone = reader.GetString(reader.GetOrdinal("phone")),
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

        public async Task<bool> UpdateCustomerAsync(int customerId, CustomerRequest customer)
        {
            try
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(
                    "SELECT public.UpdateCustomer(@customerid ,@firstname, @lastname, @dob, @email, @phone, @address);", conn);

                cmd.Parameters.AddWithValue("customerid", customerId);
                cmd.Parameters.AddWithValue("firstname", customer.FirstName);
                cmd.Parameters.AddWithValue("lastname", customer.LastName);
                cmd.Parameters.AddWithValue("dob", NpgsqlTypes.NpgsqlDbType.Date, customer.Dob.Date);
                cmd.Parameters.AddWithValue("email", customer.Email);
                cmd.Parameters.AddWithValue("phone", customer.Phone);
                cmd.Parameters.AddWithValue("address", customer.Address);

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
