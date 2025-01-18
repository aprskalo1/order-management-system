using CustomerService.Data;
using CustomerService.Exceptions;
using CustomerService.Mapping;
using CustomerService.Messaging.RPC;
using CustomerService.Repositories;
using CustomerService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options => { options.Filters.Add<CustomerExceptionFilter>(); });

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer("server=.;Database=CustomerData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService.Services.CustomerService>();

builder.Services.AddSingleton<CustomerRpcServer>();
builder.Services.AddHostedService<RpcServerHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();