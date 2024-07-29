using MyFirstService.Dto;
using MyFirstService.Util;

namespace MyFirstService.Repository
{

    public class UserRepository
    {
        private readonly List<User> _users;

        public UserRepository()
        {
            _users = new List<User>
            {
                new User() { Id = Guid.NewGuid(), Name = "GuokasWu", PasswordHash = CryptographyHelper.GetHashString("111111") , Email = "guokaswu@gmail.com", country = "FR", localeID = "fr-ch"},
                new User() { Id = Guid.NewGuid(), Name = "George", PasswordHash = CryptographyHelper.GetHashString("222222"), Email = "george@outlook.com", country = "EN", localeID = "en-us" },
                new User() { Id = Guid.NewGuid(), Name = "Mishelle", PasswordHash = CryptographyHelper.GetHashString("333333"), Email = "mishelle@gmail.com", country = "NL", localeID = "nl-nl" }
            };
        }

        public User? QueryUser(string name)
        {
            return _users.Find(x => x.Name.Equals(name));
        }

        internal User? QueryUserById(string id)
        {
            return _users.Find(x => x.Id.ToString().Equals(id));
        }
    }
}
