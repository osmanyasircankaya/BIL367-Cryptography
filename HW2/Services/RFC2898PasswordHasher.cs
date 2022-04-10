using System;
using System.Security.Cryptography;
using HW2.Interfaces;

namespace HW2.Services
{
    public class RFC2898PasswordHasher : IPasswordHasher
    {
        private const int saltByteSize = 24;
        private const int hashByteSize = 24;
        private const int hashingIterationsCount = 10101;

        public GeneratedPasswordHash GeneratePasswordHash(string password)
        {
            var result = new GeneratedPasswordHash();
            var salt = GenerateSalt();
            var hashedPassword = ComputeHash(password, salt);

            result.PasswordSalt = ToBase64(salt);
            result.PasswordHash = ToBase64(hashedPassword);

            return result;
        }

        public bool ValidatePassword(string password, string passwordSalt, string passwordHash)
        {
            var salt = FromBase64(passwordSalt);

            var computedHash = ComputeHash(password, salt);

            return ToBase64(computedHash) == passwordHash;
        }

        #region Helpers

        private static byte[] GenerateSalt()
        {
            using (RandomNumberGenerator saltGenerator = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[saltByteSize];
                saltGenerator.GetBytes(salt);
                return salt;
            }
        }

        private static byte[] ComputeHash(string password, byte[] salt)
        {
            using (Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(password, salt))
            {
                hashGenerator.IterationCount = hashingIterationsCount;
                return hashGenerator.GetBytes(hashByteSize);
            }
        }

        private string ToBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        private byte[] FromBase64(string data)
        {
            return Convert.FromBase64String(data);
        }

        #endregion
    }
}