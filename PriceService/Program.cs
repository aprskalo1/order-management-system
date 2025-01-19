using Microsoft.EntityFrameworkCore;
using PriceService.Data;
using PriceService.Exceptions;
using PriceService.Mapping;
using PriceService.Repositories;
using PriceService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options => { options.Filters.Add<ProductExceptionFilter>(); });

builder.Services.AddDbContext<PriceDbContext>(options =>
    options.UseSqlServer("server=.;Database=OrderData;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPriceService, PriceService.Services.PriceService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();