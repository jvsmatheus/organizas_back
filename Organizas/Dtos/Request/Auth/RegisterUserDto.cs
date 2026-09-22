namespace Organizas.Dtos.Request.Auth
{
    public record RegisterUserDto(
        string Username,
        string Email,
        string Password
    );
}
