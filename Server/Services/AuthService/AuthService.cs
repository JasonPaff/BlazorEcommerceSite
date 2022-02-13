using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        // inject database context, configuration
        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // register user
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            // check for existence
            if (await UserExists(user.Email))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            // create password hash
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            // store values in user
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // add user to database
            _context.Users.Add(user);

            // update database
            await _context.SaveChangesAsync();

            // return user id
            return new ServiceResponse<int> {Data = user.Id};
        }

        // return true if user exists
        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email.ToLower().Equals(email.ToLower()));
        }

        // login user
        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response = new ServiceResponse<string>();

            // find the user
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));

            // no user found
            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            // wrong password
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            // user found and correct password
            else
                response.Data = CreateToken(user);

            return response;
        }

        // create json web token
        private string CreateToken(User user)
        {
            // create claims
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };

            // get secret key from config settings
            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            // security credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // create security token
            var token = new JwtSecurityToken(
                claims: claims, 
                expires : DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            // create json web token
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            
            // hash attempted password
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            // compare attempted password to saved password
            return computedHash.SequenceEqual(passwordHash);
        }

        // hash password
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // generate key
            using var hmac = new HMACSHA512();

            // salt 
            passwordSalt = hmac.Key;

            // hash
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}