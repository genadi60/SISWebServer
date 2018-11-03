namespace MishMashWebApp.Services.Contracts
{
    using Data;
    using InputModels.Users;
    using ViewModels.Users;

    public interface IUserService
    {
        bool Create(RegisterInputModel model, MishMashDbContext context);

        bool UserIsAuthenticated(LoginInputModel model, MishMashDbContext context);

        bool Find(string username, string password, MishMashDbContext context);

        UserViewModel Profile(string username, MishMashDbContext context);

        int GetUserId(string username, MishMashDbContext context);
    }
}
