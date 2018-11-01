using CakesWebApp.Data;
using SIS.MvcFramework;

namespace CakesWebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController() 
        {
            Db = new CakesDbContext();
        }
        
        protected CakesDbContext Db { get; }
    }
}
