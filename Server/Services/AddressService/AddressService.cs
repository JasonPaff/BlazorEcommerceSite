using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Server.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;

        // inject database context, auth service
        public AddressService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        
        // return user address
        public async Task<ServiceResponse<Address>> GetAddress()
        {
            var userId = _authService.GetUserId();
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

            return new ServiceResponse<Address> {Data = address};
        }

        // add or update user address
        public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
        {
            var response = new ServiceResponse<Address>();
            var dbAddress = (await GetAddress()).Data;

            if (dbAddress is null)
            {
                // add new address
                address.UserId = _authService.GetUserId();
                _context.Addresses.Add(address);
                
                response.Data = address;
            }
            else
            {
                // update existing address
                dbAddress.FirstName = address.FirstName;
                dbAddress.LastName = address.LastName;
                dbAddress.Street = address.Street;
                dbAddress.City = address.City;
                dbAddress.State = address.State;
                dbAddress.ZipCode = address.ZipCode;
                dbAddress.Country = address.Country;
                
                response.Data = dbAddress;
            }
            
            // update database
            await _context.SaveChangesAsync();

            return response;
        }
    }
}