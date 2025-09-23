using BankAPI.Messaging;
using BankAPI.Models.CustomExceptions;
using BankAPI.Models.DB;
using BankAPI.Models.Request;
using BankAPI.Repositories.Implementations;
using BankAPI.Repositories.Interfaces;
using BankAPI.Services.Interfaces;

namespace BankAPI.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public CustomerService(ICustomerRepository customerRepository, IRabbitMqPublisher rabbitMqPublisher)
        {
            _customerRepository = customerRepository;
            _rabbitMqPublisher = rabbitMqPublisher;
        }
        public async Task<int> CreateCustomerAsync(CustomerRequest customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName))
                throw new ArgumentException("First Name required.");
            if (customer.Dob == default)
                throw new ArgumentException("Date of birth is required.");
            if (string.IsNullOrWhiteSpace(customer.Email))
                throw new ArgumentException("Email required.");
            if (string.IsNullOrWhiteSpace(customer.Phone))
                throw new ArgumentException("Phone required.");
            if (string.IsNullOrWhiteSpace(customer.Address))
                throw new ArgumentException("Address required.");
            
            var customerId = await _customerRepository.CreateCustomerAsync(customer);

            if (customerId > 0)
            {
                var message = $"Customer created: {customer.FirstName} ID: {customerId}";
                await _rabbitMqPublisher.PublishMessage(message);
            }

            return customerId;
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var result = await _customerRepository.DeleteCustomerAsync(customerId);

            string message;

            if (result)
                message = $"Customer deleted: {customerId}";
            else
                message = $"Customer NOT deleted: {customerId}";

            return result;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllCustomersAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    return null;

                var customer = await _customerRepository.GetCustomerByIdAsync(customerId);

                if (customer == null)
                    return null;

                customer.FirstName = customer.FirstName.Trim();
                customer.LastName = customer.LastName.Trim();
                // email verificaiton logic
                // phone number verificaiton logic
                customer.Address = customer.Address.Trim();

                return customer;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateCustomerAsync(int customerId, CustomerRequest customer)
        {
            if (customerId <= 0)
                throw new ServiceException("Invalid Customer ID");

            var result = await _customerRepository.UpdateCustomerAsync(customerId, customer);

            var message = $"Customer updated: {customer.FirstName}, ID: {customerId}";
            await _rabbitMqPublisher.PublishMessage(message);

            return result;
        }
    }
}
