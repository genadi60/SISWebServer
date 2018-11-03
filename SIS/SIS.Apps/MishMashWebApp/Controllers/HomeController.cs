namespace MishMashWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using ViewModels.Channels;
    using SIS.HTTP.Responses.Contracts;
    using ViewModels.Home;

    public class HomeController : BaseController
    {
        public IHttpResponse Index()
        {
            var user = Db.Users.FirstOrDefault(u => u.Username.Equals(User.Username));

            if (user != null)
            {
                var channelModels = Db.Channels
                .Select(c => new BaseChannelViewModel
                {
                    Id = c.Id,
                    FollowersCount = c.Followers.Count,
                    ChannelTagsIds = c.Tags.Select(t => t.TagId).ToList(),
                    Type = c.Type,
                    Name = c.Name,
                    FollowersIds = c.Followers.Select(f => f.UserId).ToList()
                }).ToList();

                var followedChannels = channelModels
                    .Where(c => c.FollowersIds.Contains(user.Id))
                    .ToList();

                var followedChannelsTags = new List<int>();
                followedChannels.ForEach(c => followedChannelsTags.AddRange(c.ChannelTagsIds));

                var suggestedChannels = channelModels
                    .Where(c => !followedChannels.Select(fc => fc.Id).ToList().Contains(c.Id)
                                && c.ChannelTagsIds.Any(ct => followedChannelsTags.Contains(ct)))
                    .ToList();

                var unionChannelsIds =
                    followedChannels.Select(c => c.Id).Concat(suggestedChannels.Select(c => c.Id)).ToList();

                var seeOtherChannels = channelModels.Where(c => !unionChannelsIds.Contains(c.Id)).ToList();

                var model = new LoggedInIndexViewModel
                {
                    Role = user.Role.ToString(),

                    YourChannels = followedChannels,
                    SuggestedChannels = suggestedChannels,
                    SeeOtherChannels = seeOtherChannels
                };

                return View("Home/LoggedInIndex", model);
            }
            
            return View("Home/Index");
        }
    }
}
