using ContractService.Data;
using ContractService.Exceptions;
using ContractService.Mapping;
using ContractService.Messaging.RPC;
using ContractService.Repositories;
using ContractService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options => { options.Filters.Add<ContractExceptionFilter>(); });

builder.Services.AddDbContext<ContractDbContext>(options =>
    options.UseSqlServer("server=.;Database=ContractData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractService, ContractService.Services.ContractService>();

builder.Services.AddSingleton<CustomerRpcClient>();
builder.Services.AddHostedService<RpcClientHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();