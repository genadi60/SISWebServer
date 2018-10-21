using CakesWebApp.ViewModels.Account;

namespace CakesWebApp.Services.Contracts
{
    public interface IUserService
    {
        bool Create(RegisterViewModel model);

        bool Find(string username, string password);

        ProfileViewModel Profile(string username);

        int? GetUserId(string username);
    }
}
