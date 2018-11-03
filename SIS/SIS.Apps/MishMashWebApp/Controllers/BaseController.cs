namespace MishMashWebApp.Controllers
{
    using Data;
    using SIS.MvcFramework;

    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            Db = new MishMashDbContext();
        }

        protected MishMashDbContext Db { get;}
    }
}
