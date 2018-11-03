using MishMashWebApp.Models.Enums;

namespace MishMashWebApp.ViewModels.Users
{
    public class UserViewModel
    {
        public string Title { get; set; } = "My Profile";

        public string Username { get; set; }

        public string Email { get; set; }

        public Role Role { get; set; }
    }
}
