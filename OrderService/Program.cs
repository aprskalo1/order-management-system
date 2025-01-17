using Microsoft.EntityFrameworkCore;
using OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer("server=.;Database=OrderData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();