using HW2.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HW2.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IPasswordHasher _passwordHasher;

    public UserController(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    [HttpPut]
    [Route("Register")]
    public Task<bool> Register(string userName, string password)
    {
        //hashing password with salt
        var generatedPasswordResult = _passwordHasher.GeneratePasswordHash(password);
        
        using (var db = new ApplicationDbContext())
        {
            //saving user to database
            db.Add(new User
            {
                UserName = userName, 
                PasswordHash = generatedPasswordResult.PasswordHash,
                PasswordSalt = generatedPasswordResult.PasswordSalt
            });
            db.SaveChanges();
        }

        return Task.FromResult(true);
    }

    [HttpPost]
    [Route("Login")]
    public Task<string> Login(string userName, string password)
    {
        bool result;

        using (var db = new ApplicationDbContext())
        {
            var user = db.Users.Where(s => s.UserName == userName)
                .Select(s => new {s.PasswordSalt, s.PasswordHash}).FirstOrDefault();

            result = _passwordHasher.ValidatePassword(password, user.PasswordSalt, user.PasswordHash);
        }

        return result
            ? Task.FromResult("Başarıyla giriş yapıldı.")
            : Task.FromResult("Kullanıcı adı ve şifre uyuşmuyor.");
    }

    [HttpPost]
    [Route("MD5")]
    public Task<bool> MD5()
    {
        var input = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Text.txt"));

        char GetLetter()
        {
            const string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
            var rand = new Random();
            var num = rand.Next(0, chars.Length);
            return chars[num];
        }

        using (var db = new ApplicationDbContext())
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var i = 0;
                var charArray = input.ToCharArray();

                do
                {
                    var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    var hashBytes = md5.ComputeHash(inputBytes);
                    var result = Convert.ToHexString(hashBytes);

                    db.Add(new Hash()
                    {
                        Text = input,
                        HashedText = result
                    });

                    charArray[i] = GetLetter();
                    input = new string(charArray);
                    i++;
                } while (i < charArray.Length);

                db.SaveChanges();
            }
        }

        return Task.FromResult(true);
    }
}