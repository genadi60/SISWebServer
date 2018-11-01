namespace SIS.MvcFramework.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorViewModel(string message)
        {
            ErrorMessage = message;
        }

        public string Title { get; set; } = "Error";

        public string ErrorMessage { get; set; }
    }
}
