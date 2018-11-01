using System.Collections.Generic;
using System.Linq;
using CakesWebApp.Data;
using CakesWebApp.Services.Contracts;
using CakesWebApp.ViewModels.Product;
using CakesWebApp.ViewModels.Shopping;
using CakesWebApp.ViewModels.User;

namespace CakesWebApp.Services
{
    public class UserService : IUserService
    {
        public bool Find(string username, string password, CakesDbContext context)
        {
            using (var db = context)
            {
                return db
                    .Users
                    .Any(u => u.Username == username && u.Password == password);
            }
        }

        public ProfileViewModel Profile(string username, CakesDbContext context)
        {
            using (var db = context)
            {
                var model = db.Users
                    .Where(u => u.Username == username)
                    .Select(u => new ProfileViewModel
                    {
                        Name = u.Name,
                        Username = u.Username,
                        RegistrationDate = u.DateOfRegistration
                    })
                    .FirstOrDefault();

                var ordersCount = db.Orders.Count(o => o.User.Username.Equals(username));

                if (model != null)
                {
                    model.TotalOrders = ordersCount;
                    
                }
                return model;
            }
        }
        
        public int GetUserId(string username, CakesDbContext context)
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

        public ICollection<OrderViewModel> GetMyOrders(string username, CakesDbContext context)
        {
            using (context)
            {
                var orders = context.Orders
                    .Where(o => o.User.Username.Equals(username))
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        DateOfCreation = o.DateOfCreation,
                        Products = o.Products.Select(op => new ProductViewModel
                        {
                            Id = op.ProductId,
                            Name = op.Product.Name,
                            Price = op.Product.Price
                        }).ToList(),
                        TotalPrice = o.Products.Sum(p => p.Product.Price)
                    })
                    .ToList();

                return orders;
            }
        }
    }
}
