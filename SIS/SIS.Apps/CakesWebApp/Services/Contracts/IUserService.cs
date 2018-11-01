using System.Collections.Generic;
using CakesWebApp.Data;
using CakesWebApp.ViewModels.Shopping;
using CakesWebApp.ViewModels.User;

namespace CakesWebApp.Services.Contracts
{
    public interface IUserService
    {
        bool Find(string username, string password, CakesDbContext context);

        ProfileViewModel Profile(string username, CakesDbContext context);

        ICollection<OrderViewModel> GetMyOrders(string username, CakesDbContext context);

        int GetUserId(string username, CakesDbContext context);
    }
}
