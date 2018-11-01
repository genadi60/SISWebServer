namespace CakesWebApp.ViewModels.User
{
    using System;

    public class ProfileViewModel
    {
        public string Title { get; set; } = "My Profile";

        public string Name { get; set; }

        public string Username { get; set; }

        public DateTime RegistrationDate { get; set; }

        public virtual int TotalOrders { get; set; }
    }
}
