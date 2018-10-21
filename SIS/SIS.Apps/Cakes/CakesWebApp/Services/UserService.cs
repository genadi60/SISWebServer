using CakesWebApp.ViewModels.Account;

namespace CakesWebApp.Services
{
    using System.Linq;

    using Contracts;
    using Data;
    using Models;

    public class UserService : IUserService
    {
        public bool Create(RegisterViewModel model)
        {
            using (var db = new CakesDbContext())
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

        public bool Find(string username, string password)
        {
            using (var db = new CakesDbContext())
            {
                return db
                    .Users
                    .Any(u => u.Username == username && u.Password == password);
            }
        }

        public ProfileViewModel Profile(string username)
        {
            using (var db = new CakesDbContext())
            {
                return db
                    .Users
                    .Where(u => u.Username == username)
                    .Select(u => new ProfileViewModel
                    {
                        Username = u.Username,
                        RegistrationDate = u.DateOfRegistration,
                        TotalOrders = u.Orders.Count()
                    })
                    .FirstOrDefault();
            }
        }

        public int? GetUserId(string username)
        {
            using (var db = new CakesDbContext())
            {
                var id = db
                    .Users
                    .Where(u => u.Username == username)
                    .Select(u => u.Id)
                    .FirstOrDefault();

                return id != 0 ? (int?)id : null;
            }
        }
    }
}
