namespace CakesWebApp.ViewModels.Shopping
{
    using System;
    using System.Collections.Generic;

    using Product;

    public sealed class OrderViewModel
    {
        public OrderViewModel()
        {
            Products = new List<ProductViewModel>();
        }

        public string Title => $"Order {Id}";

        public int Id { get; set; }

        public DateTime DateOfCreation;

        public ICollection<ProductViewModel> Products { get; set; }
        
        public decimal TotalPrice { get; set; }
    }
}
