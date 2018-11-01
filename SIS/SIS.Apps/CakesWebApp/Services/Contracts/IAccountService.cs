using CakesWebApp.Data;
using CakesWebApp.InputModels.Account;

namespace CakesWebApp.Services.Contracts
{
    public interface IAccountService
    {
        bool Create(RegisterInputModel model, CakesDbContext context);

        bool UserLogin(LoginInputModel model, CakesDbContext context);
    }
}
