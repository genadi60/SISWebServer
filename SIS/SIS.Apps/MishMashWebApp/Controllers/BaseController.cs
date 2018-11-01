using MishMashWebApp.Data;
using SIS.MvcFramework;

namespace MishMashWebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            Db = new MishMashDbContext();
        }

        protected MishMashDbContext Db { get;}
    }
}
