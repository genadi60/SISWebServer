using CakesWebApp.Data;
using SIS.MvcFramework.Services.Contracts;

namespace CakesWebApp.Services.Contracts
{
    using SIS.MvcFramework.Services;
    using InputModels.Account;
    
    public interface IAccountService
    {
        bool Create(RegisterInputModel model, CakesDbContext context);

        bool UserLogin(LoginInputModel model, CakesDbContext context);
    }
}
