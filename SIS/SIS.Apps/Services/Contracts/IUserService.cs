using System.Collections.Generic;
using CakesWebApp.Data;
using CakesWebApp.ViewModels.Shopping;

namespace CakesWebApp.Services.Contracts
{
    using InputModels.Account;
    using ViewModels.User;

    public interface IUserService
    {
        bool Find(string username, string password, CakesDbContext context);

        ProfileViewModel Profile(string username, CakesDbContext context);

        ICollection<OrderViewModel> GetMyOrders(string username, CakesDbContext context);

        int GetUserId(string username, CakesDbContext context);
    }
}
