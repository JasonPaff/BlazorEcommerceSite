using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;

        // inject database context
        public AuthService(DataContext context)
        {
            _context = context;
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