namespace CakesWebApp.ViewModels.Shopping
{
    using System;
    using System.Collections.Generic;

    using Models;

    public class OrderViewModel
    {
        public int Id { get; set; }

        public DateTime DateOfCreation;

        public virtual ICollection<OrderProduct> Products { get; set; } = new HashSet<OrderProduct>();
    }
}
