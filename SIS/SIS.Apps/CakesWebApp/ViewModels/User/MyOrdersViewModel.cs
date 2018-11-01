using System.Collections.Generic;
using CakesWebApp.ViewModels.Shopping;

namespace CakesWebApp.ViewModels.User
{
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
