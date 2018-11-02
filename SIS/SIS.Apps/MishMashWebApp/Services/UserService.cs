using System.Linq;
using MishMashWebApp.Data;
using MishMashWebApp.InputModels.Users;
using MishMashWebApp.Models;
using MishMashWebApp.Services.Contracts;
using MishMashWebApp.ViewModels.Users;
using SIS.MvcFramework.Services.Contracts;

namespace MishMashWebApp.Services
{
    public class UserService : IUserService
    {
        private readonly IHashService _hashService;

        public UserService(IHashService hashService)
        {
            _hashService = hashService;
        }

        public bool Find(string username, string password, MishMashDbContext context)
        {
            using (var db = context)
            {
                return db
                    .Users
                    .Any(u => u.Username == username && u.Password == password);
            }
        }

        public ProfileViewModel Profile(string username, MishMashDbContext context)
        {
            using (var db = context)
            {
                var model = db.Users
                    .Where(u => u.Username == username)
                    .Select(u => new ProfileViewModel
                    {
                        Username = u.Username,
                        Email = u.Email
                    })
                    .FirstOrDefault();

                return model;
            }
        }
        
        public int GetUserId(string username, MishMashDbContext context)
        {
            using (var db = context)
            {
                var id = db
                    .Users
                    .Where(u => u.Username == username)
                    .Select(u => u.Id)
                    .First();

                return id;
            }
        }

        public bool Create(RegisterInputModel model, MishMashDbContext context)
        {
            using (var db = context)
            {
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    return false;
                }

                var user = new User
                {
                    Username = model.Username,
                    Password = _hashService.Hash(model.Password),
                    Email = model.Email,
                    Role = model.Role
                };

                db.Add(user);
                db.SaveChanges();

                return true;
            }
        }

        public bool UserLogin(LoginInputModel model, MishMashDbContext context)
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
