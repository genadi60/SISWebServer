namespace CakesWebApp.ViewModels.Product
{
    public class ProductDetailsViewModel
    {
        public string Title => $"{Name}";

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}
