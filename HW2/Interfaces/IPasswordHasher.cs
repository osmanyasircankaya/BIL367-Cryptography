namespace HW2.Interfaces
{
    public interface IPasswordHasher
    {
        GeneratedPasswordHash GeneratePasswordHash(string password);
        bool ValidatePassword(string password, string passwordSalt, string passwordHash);
    }
    public class GeneratedPasswordHash
    {
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
    }
}
