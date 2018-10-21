namespace CakesWebApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;
    using Data;
    using Models;
    
    public class ShoppingService : IShoppingService
    {
        public void CreateOrder(int userId, IEnumerable<int> productIds)
        {
            using (var db = new CakesDbContext())
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

                db.Add(order);
                db.SaveChanges();
            }
        }
    }
}
