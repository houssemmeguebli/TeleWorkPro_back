using System.Threading.Tasks;
using TTProject.Core.Entities;
using TTProject.Core.Interfaces;

namespace TTProject.Application.Services
{
    public class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<(string firstName, string lastName)> GetUserByNameAsync(long userID)
        {
            return await _userRepository.GetUserByNameAsync(userID);
        }
    }
}
