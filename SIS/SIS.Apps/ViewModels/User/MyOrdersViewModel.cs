namespace CakesWebApp.ViewModels.User
{
    using System.Collections.Generic;

    using Shopping;

    public class MyOrdersViewModel
    {
        public MyOrdersViewModel()
        {
            MyOrders = new List<OrderViewModel>();
        }
        
        public string Title { get; set; } = "Your Orders";

        public virtual ICollection<OrderViewModel> MyOrders { get; set; }

    }
}
