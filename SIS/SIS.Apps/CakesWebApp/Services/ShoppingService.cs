using System;
using System.Collections.Generic;
using System.Linq;
using CakesWebApp.Data;
using CakesWebApp.Models;
using CakesWebApp.Services.Contracts;
using CakesWebApp.ViewModels.Product;
using CakesWebApp.ViewModels.Shopping;

namespace CakesWebApp.Services
{
    public class ShoppingService : IShoppingService
    {
        public void CreateOrder(int userId, IEnumerable<int> productIds, CakesDbContext context)
        {
            using (var db = context)
            {
                var order = new Order
                {
                    UserId = userId,
                    DateOfCreation = DateTime.UtcNow,
                    Products = productIds
                        .Select(id => new OrderProduct
                        {
                            ProductId = id
                        })
                        .ToList()
                };

                db.Orders.Add(order);
                db.SaveChanges();
            }
        }

        public OrderViewModel GetOrderViewModel(int id, CakesDbContext context)
        {
            using (var db = context)
            {
                return db.Orders
                    .Where(o => o.Id == id)
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        DateOfCreation = o.DateOfCreation,
                        Products = o.Products.Select(op => new ProductViewModel
                            {
                                Id = op.ProductId,
                                Name = op.Product.Name,
                                Price = op.Product.Price
                            })
                            .ToList(),
                        TotalPrice = o.Products.Sum(op => op.Product.Price)
                    })
                    .FirstOrDefault();
            }
        }
    }
}
