using MishMashWebApp.Models.Enums;

namespace MishMashWebApp.InputModels.Users
{
    public class RegisterInputModel
    {
        public string Title { get; set; } = "Register";

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public Role Role { get; set; } = Role.User;
    }
}
