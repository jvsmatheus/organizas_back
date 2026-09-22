using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Organizas.Dtos.Request.Auth;
using Organizas.Entities;

namespace Organizas.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<RegisterUserDto> _registerValidator;

        public AuthService(
            UserManager<User> userManager,
            IValidator<RegisterUserDto> registerValidator)
        {
            _userManager = userManager;
            _registerValidator = registerValidator;
        }

        public async Task Register(RegisterUserDto request, CancellationToken cancellationToken)
        {
            await _registerValidator.ValidateAndThrowAsync(
                request,
                cancellationToken
            );

            var email = request.Email.Trim();

            var user = new User
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(error =>
                    new FluentValidation.Results.ValidationFailure(string.Empty, error.Description)
                    {
                        ErrorCode = error.Code
                    }
                );

                throw new ValidationException(errors);
            }
        }
    }
}
