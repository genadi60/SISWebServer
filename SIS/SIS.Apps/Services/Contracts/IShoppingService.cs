using System;
using CakesWebApp.Data;
using CakesWebApp.ViewModels.Product;
using CakesWebApp.ViewModels.Shopping;

namespace CakesWebApp.Services.Contracts
{
    using System.Collections.Generic;

    public interface IShoppingService
    {
        void CreateOrder(int userId, IEnumerable<int> productIds, CakesDbContext context);

        OrderViewModel GetOrderViewModel(int id, CakesDbContext context);
    }
}
