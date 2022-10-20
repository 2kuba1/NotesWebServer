using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NotesWebServer.Entities;
using NotesWebServer.Models;

namespace NotesWebServer.Services
{
    public interface IAccountService
    {
        string Login(LoginDto user);
        void Register(CreateUserDto user);
    }

    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountService(IConfiguration configuration, AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public string Login(LoginDto userDto)
        {
            var user = _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == userDto.Username);

            if (user is null)
            {
                return "Username or password is incorrect";
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, userDto.HashedPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                return "Username or password is incorrect";
            }

            var issuer = _configuration.GetValue<string>("Jwt:Issuer");
            var audience = _configuration.GetValue<string>("Jwt:Audience");
            var key = Encoding.ASCII.GetBytes
                (_configuration.GetValue<string>("Jwt:Key"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, $"{user.Username}"),
                    new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                }),
                Expires = DateTime.Now.AddDays(_configuration.GetValue<int>("Jwt:ExpireDays")),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public void Register(CreateUserDto user)
        {
            var newUser = new User()
            {
                Username = user.Username,
                RoleId = 1
            };

            var hashedPassword = _passwordHasher.HashPassword(newUser, user.HashedPassword);

            newUser.HashedPassword = hashedPassword;
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
        }
    }
}
