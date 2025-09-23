using BankAPI.Messaging;
using BankAPI.Repositories.Implementations;
using BankAPI.Repositories.Interfaces;
using BankAPI.Services.Implementations;
using BankAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Registering Repositories
builder.Services.AddScoped < IBankRepository, BankRepository >();
builder.Services.AddScoped < ICustomerRepository, CustomerRepository >();
builder.Services.AddScoped < IAccountRepository, AccountRepository >();

// Registering Services
builder.Services.AddScoped < IBankService, BankService >();
builder.Services.AddScoped < ICustomerService, CustomerService >();
builder.Services.AddScoped < IAccountService, AccountService >();

// Registering Rabbit MQ
 builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
