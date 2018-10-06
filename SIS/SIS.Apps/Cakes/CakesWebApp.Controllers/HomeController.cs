using System.Text;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;

namespace CakesWebApp.Controllers
{
    public class HomeController : BaseController
    {
        public IHttpResponse Index(IHttpRequest request)
        {
            return View("index");
        }

        public IHttpResponse Hello(IHttpRequest request)
        {
            var view = View("hello");
            var userName = GetUserName(request);
            if (userName != null)
            {
                var content = Encoding.UTF8.GetString(view.Content).Replace("@Render", userName);
                view.Content = Encoding.UTF8.GetBytes(content);
            }
            
            return view;
        }
    }
}
