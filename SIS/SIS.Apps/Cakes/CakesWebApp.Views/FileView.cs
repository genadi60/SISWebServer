namespace CakesWebApp.Views
{
    public class FileView : IView
    {
        private readonly string _htmlString;

        public FileView(string htmlString)
        {
            _htmlString = htmlString;
        }

        public string View()
        {
            return _htmlString;
        }
    }
}
