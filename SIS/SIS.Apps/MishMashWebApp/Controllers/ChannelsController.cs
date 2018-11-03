namespace MishMashWebApp.Controllers
{
    using System.Linq;

    using InputModels.Channels;
    using Models.Enums;
    using Services.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.ViewModel;


    public class ChannelsController : BaseController
    {
        private readonly IChannelService _channelService;

        public ChannelsController(IChannelService channelService)
        {
            _channelService = channelService;
        }
        
        public IHttpResponse Details(ChannelInputModel model)
        {
            if (User == null)
            {
                return Redirect("/Users/Login");
            }

            var viewModel = _channelService.GetChannelViewModel(model, Db);

            return View("/Channels/Details", viewModel);
        }

        public IHttpResponse Followed()
        {
            if (User == null)
            {
                return Redirect("/Users/Login");
            }

            var viewModel = _channelService.GetFollowedChannels(User.Username, Db);

            return View("/Channels/Followed", viewModel);

        }
        
        public IHttpResponse Follow(ChannelInputModel model)
        {
            if (User == null)
            {
                return Redirect("/Users/Login");
            }

            var chanelId = model.Id;
            
            if (_channelService.IsFollow(User.Username, chanelId, Db))
            {
                return Redirect("/Channels/Followed");
            }

            var errorMessage = "The Channel can not to be follow!";
            return View("/Error", new ErrorViewModel(errorMessage));
        }

        public IHttpResponse Unfollow(ChannelInputModel model)
        {
            if (User == null)
            {
                return Redirect("/Users/Login");
            }

            var chanelId = model.Id;
            
            if (_channelService.IsUnfollow(User.Username, chanelId, Db))
            {
                return Redirect("/Channels/Followed");
            }

            return Redirect("/Channels/Followed");
        }

        public IHttpResponse Create()
        {
            var user = Db.Users.FirstOrDefault(u => u.Username.Equals(User.Username));
            if (user == null || user.Role != Role.Admin)
            {
                Redirect("/Users/Login");
            }
            return View("/Channels/Create");
        }

        [HttpPost("/Channels/Create")]
        public IHttpResponse Create(CreateChannelsInputModel model)
        {
            var user = Db.Users.FirstOrDefault(u => u.Username.Equals(User.Username));
            if (user == null || user.Role != Role.Admin)
            {
                Redirect("/Users/Login");
            }

            var channelId = _channelService.Create(model, Db);

            if (channelId == 0)
            {
                var errorMessage = "Invalid Channel";
                return View("/Error", new ErrorViewModel(errorMessage));
            }

            return Redirect($"/Channels/Details?id={channelId}");
        }
    }
}
