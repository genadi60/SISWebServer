using System.Linq;
using CakesWebApp.Data;
using CakesWebApp.InputModels.Account;
using CakesWebApp.Models;
using CakesWebApp.Services.Contracts;
using SIS.MvcFramework.Services.Contracts;

namespace CakesWebApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHashService _hashService;

        public AccountService(IHashService hashService)
        {
            _hashService = hashService;
        }

        public bool Create(RegisterInputModel model, CakesDbContext context)
        {
            using (var db = context)
            {
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    return false;
                }

                var user = new User
                {
                    Name = model.Name,
                    Username = model.Username,
                    Password = model.Password
                };

                db.Add(user);
                db.SaveChanges();

                return true;
            }
        }

        public bool UserLogin(LoginInputModel model, CakesDbContext context)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password)
                                                          || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return false;
            }

            var hashedPassword = _hashService.Hash(model.Password);

            using (context)
            {
                var user = context.Users.FirstOrDefault(u => u.Username.Equals(model.Username));

                bool isEqual = user != null && user.Username.Equals(model.Username);

                if (!context.Users.Any(u => u.Password.Equals(hashedPassword)) || !isEqual)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
