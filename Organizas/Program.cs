using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.Auth;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Dtos.Request.ShoppingListItem;
using Organizas.Entities;
using Organizas.Infra.Db;
using Organizas.Infra.Errors;
using Organizas.Services;
using Organizas.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

#region Validation
// ShoppingListItem
builder.Services.AddScoped<IValidator<CreateShoppingListItemDto>, CreateShoppingListItemValidator>();
builder.Services.AddScoped<IValidator<UpdateShoppingListItemDto>, UpdateShoppingListItemValidator>();

// ShoppingList
builder.Services.AddScoped<IValidator<CreateShoppingListDto>, CreateShoppingListValidator>();
builder.Services.AddScoped<IValidator<UpdateShoppingListDto>, UpdateShoppingListValidator>();

// Auth
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserValidator>();
#endregion

#region Depency Injection
// Application dependency injection
builder.Services.AddScoped<ShoppingListItemService>();
builder.Services.AddScoped<ShoppingListService>();
builder.Services.AddScoped<AuthService>();
#endregion

// Exception handler
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// DbContext
builder.Services.AddDbContext<OrganizasDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Organizas"))
);

// Identity
builder.Services
    .AddIdentityCore<User>(options => { options.User.RequireUniqueEmail = true; })
    .AddEntityFrameworkStores<OrganizasDbContext>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<OrganizasDbContext>();

// Swager
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger();

app.MapControllers();

app.MapIdentityApi<User>();

app.Run();
