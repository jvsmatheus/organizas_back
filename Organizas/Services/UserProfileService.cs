using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Organizas.Dtos.Request.UserProfile;
using Organizas.Dtos.Response.UserProfile;
using Organizas.Infra.Db;
using System.ComponentModel.DataAnnotations;

namespace Organizas.Services
{
    public sealed class UserProfileService
    {
        private readonly OrganizasDbContext _context;
        private readonly IValidator<UpdateUserProfileDto> _validator;

        public UserProfileService(
            OrganizasDbContext dbContext,
            IValidator<UpdateUserProfileDto> validator
        )
        {
            _context = dbContext;
            _validator = validator;
        }

        public async Task<UserProfileResponseDto?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .Where(perfil => perfil.UserId == userId)
                .Select(perfil => new UserProfileResponseDto(perfil.Name))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<UserProfileResponseDto> SaveAsync(string userId, UpdateUserProfileDto dto, CancellationToken cancellationToken = default)
        {
            await _validator.ValidateAndThrowAsync(dto, cancellationToken);

            var perfil = await _context.UserProfiles.SingleOrDefaultAsync(perfil => perfil.UserId == userId, cancellationToken);

            if (perfil is null)
            {
                perfil = new Entities.UserProfile
                {
                    UserId = userId,
                    Name = dto.Name.Trim()
                };

                _context.UserProfiles.Add(perfil);
            }
            else
            {
                perfil.Name = dto.Name.Trim();
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new UserProfileResponseDto(perfil.Name);
        }
    }
}
