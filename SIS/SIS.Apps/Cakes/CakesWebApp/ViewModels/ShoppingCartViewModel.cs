namespace CakesWebApp.ViewModels
{
    using System.Collections.Generic;


    public class ShoppingCartViewModel
    {
        public const string SessionKey = "%^Current_Shopping_Cart^%";

        public IList<int> ProductIds { get; private set; } = new List<int>();
    }
}
