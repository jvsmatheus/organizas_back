namespace Organizas.Dtos.Request.User
{
    public record RegisterUserRequestDto(
        string Email,
        string Password,
        string FirstName,
        string? LastName,
        string? PhoneNumber,
        DateOnly? DateOfBirth
    );
}
