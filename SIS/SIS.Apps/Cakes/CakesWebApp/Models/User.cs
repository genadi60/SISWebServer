namespace CakesWebApp.Models
{
    using System;
    using System.Collections.Generic;


    public class User : BaseModel<int>
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;

        public virtual ICollection<OrderProduct> Orders { get; set; } = new HashSet<OrderProduct>();
    }
}
