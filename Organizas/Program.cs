using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.ShoppingList;
using Organizas.Dtos.Request.ShoppingListItem;
using Organizas.Dtos.Request.UserProfile;
using Organizas.Entities;
using Organizas.Infra.Db;
using Organizas.Infra.Email;
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

// UserProfile
builder.Services.AddScoped<IValidator<UpdateUserProfileDto>, UpdateUserProfileValidator>();
#endregion

#region Depency Injection
// Application dependency injection
builder.Services.AddScoped<ShoppingListItemService>();
builder.Services.AddScoped<ShoppingListService>();
builder.Services.AddScoped<UserProfileService>();
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
    .AddIdentityApiEndpoints<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 10;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<OrganizasDbContext>();

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Email
builder.Services
    .AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Host),
        "Email:Host não foi carregado.")
    .Validate(o => o.Port is > 0 and <= 65535,
        "Email:Port não foi carregado ou é inválido.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Username),
        "Email:Username não foi carregado.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Password),
        "Email:Password não foi carregado.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.FromAddress),
        "Email:FromAddress não foi carregado.")
    .ValidateOnStart();

builder.Services.AddTransient<
    Microsoft.AspNetCore.Identity.IEmailSender<User>,
    IdentityEmailSender>();

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

app.MapGroup("/Auth")
    .MapIdentityApi<User>();

app.Run();
