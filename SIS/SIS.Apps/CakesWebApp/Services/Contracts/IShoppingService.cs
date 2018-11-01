using System.Collections.Generic;
using CakesWebApp.Data;
using CakesWebApp.ViewModels.Shopping;

namespace CakesWebApp.Services.Contracts
{
    public interface IShoppingService
    {
        void CreateOrder(int userId, IEnumerable<int> productIds, CakesDbContext context);

        OrderViewModel GetOrderViewModel(int id, CakesDbContext context);
    }
}
