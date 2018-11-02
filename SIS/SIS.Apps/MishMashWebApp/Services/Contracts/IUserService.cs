using MishMashWebApp.Data;
using MishMashWebApp.ViewModels.Users;
using MishMashWebApp.InputModels.Users;

namespace MishMashWebApp.Services.Contracts
{
    public interface IUserService
    {
        bool Create(RegisterInputModel model, MishMashDbContext context);

        bool UserLogin(LoginInputModel model, MishMashDbContext context);

        bool Find(string username, string password, MishMashDbContext context);

        ProfileViewModel Profile(string username, MishMashDbContext context);

        int GetUserId(string username, MishMashDbContext context);
    }
}
