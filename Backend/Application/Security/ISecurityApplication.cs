using Domain.Entities;

namespace Application.Security
{
    public interface ISecurityApplication
    {
        string GenerateToken(UserEntity user);
        void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}
