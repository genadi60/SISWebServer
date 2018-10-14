namespace CakesWebApp.Services.Contracts
{
    using ViewModels.Account;

    public interface IUserService
    {
        bool Create(RegisterViewModel model);

        bool Find(string username, string password);

        ProfileViewModel Profile(string username);

        int? GetUserId(string username);
    }
}
