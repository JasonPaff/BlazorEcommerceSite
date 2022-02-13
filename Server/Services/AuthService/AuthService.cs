using System.Linq;
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
                response.Data = "token";

            return response;
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