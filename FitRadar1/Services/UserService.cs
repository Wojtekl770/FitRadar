using FitRadar.Repositories.Interfaces;
using FitRadar.Services.Interfaces;

namespace FitRadar.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


    }
}
