using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Organizas.Dtos.Request.User;
using Organizas.Entities;

namespace Organizas.Services
{
    public sealed class AuthService
    {
        private readonly IValidator<RegisterUserRequestDto> _registerValidator;
        private readonly UserManager<User> _userManager;

        public AuthService(
            IValidator<RegisterUserRequestDto> registerValidator,
            UserManager<User> userManager
        ) {
            _registerValidator = registerValidator;
            _userManager = userManager;
        }

        public async Task RegisterAsync(RegisterUserRequestDto request, CancellationToken cancellationToken)
        {
            await _registerValidator.ValidateAndThrowAsync(request, cancellationToken);

            var user = new User()
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                CreatedOn = DateTime.UtcNow
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);
        }
    }
}
