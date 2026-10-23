using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Organizas.Dtos.Request.User;
using Organizas.Entities;
using Organizas.Infra.Email;

namespace Organizas.Services
{
    public sealed class AuthService
    {
        private readonly IValidator<RegisterUserRequestDto> _registerValidator;
        private readonly UserManager<User> _userManager;
        private readonly IdentityEmailSender _emailSender;

        public AuthService(
            IValidator<RegisterUserRequestDto> registerValidator,
            UserManager<User> userManager,
            IdentityEmailSender emailSender
        ) {
            _registerValidator = registerValidator;
            _userManager = userManager;
            _emailSender = emailSender;
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

            if (!result.Succeeded)
                throw new Exception();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _emailSender.SendConfirmationLinkAsync(user, user.Email, token);
        }
    }
}
