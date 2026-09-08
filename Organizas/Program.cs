using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Entities.Dtos.Request;
using Organizas.Infra.Db;
using Organizas.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Application dependency injection
builder.Services.AddScoped<IValidator<CreateShoppingListItemDto>, CreateShoppingListItemValidator>();

// DbContext
builder.Services.AddDbContext<OrganizasDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Organizas"))
);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
