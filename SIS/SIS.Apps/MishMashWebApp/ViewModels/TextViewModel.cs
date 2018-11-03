namespace MishMashWebApp.ViewModels
{
    public class TextViewModel
    {
        public TextViewModel(string message, string title = "Message")
        {
            Message = message;
            Title = title;
        }

        public string Title { get; set; }

        public string Message { get; set;}
    }
}
