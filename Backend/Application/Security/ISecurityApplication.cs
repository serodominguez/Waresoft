using Infrastructure.Persistences.ReadModels.User;

namespace Application.Security
{
    public interface ISecurityApplication
    {
        string GenerateToken(UserAccountReadModel user);
        void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}
