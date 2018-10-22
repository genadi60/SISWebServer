namespace CakesWebApp.ViewModels.Account
{
    using System;

    public class ProfileViewModel
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int TotalOrders { get; set; }
    }
}
