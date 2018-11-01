namespace CakesWebApp.InputModels.Product
{
    public class AddProductInputModel
    {
        public string Title => "Add Cake";
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }
    }
}
