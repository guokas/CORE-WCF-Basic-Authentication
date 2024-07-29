using MyFirstService.Dto;
using MyFirstService.Repository;

namespace MyFirstService.Application
{
    public class UserApplication
    {
        private readonly UserRepository _userRepository;
        public UserApplication()
        {
            _userRepository = new UserRepository();
        }

        public User? GetUser(string username)
        {
            return _userRepository.QueryUser(username);
        }

        public User? GetUserById(string id)
        {
            return _userRepository.QueryUserById(id);
        }
    }
}
