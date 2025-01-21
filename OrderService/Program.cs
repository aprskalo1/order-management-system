using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Exceptions;
using OrderService.Mapping;
using OrderService.Messaging;
using OrderService.Repositories;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer("server=.;Database=OrderData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();

builder.Services.AddSingleton<ProductRpcClient>();
builder.Services.AddHostedService<ProductRpcClientHostedService>();
builder.Services.AddSingleton<ContractRpcClient>();
builder.Services.AddHostedService<ContractRpcClientHostedService>();

builder.Services.AddControllers(options => options.Filters.Add<OrderExceptionFilter>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();