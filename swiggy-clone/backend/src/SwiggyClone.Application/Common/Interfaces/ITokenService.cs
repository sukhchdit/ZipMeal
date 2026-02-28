namespace SwiggyClone.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string role, string? email, string phone);
    string GenerateRefreshToken();
    string HashToken(string token);
}
