using MishMashWebApp.Data;
using MishMashWebApp.InputModels.Channels;
using MishMashWebApp.ViewModels.Channels;

namespace MishMashWebApp.Services.Contracts
{
    public interface IChannelService
    {
        ChannelViewModel GetChannelViewModel(ChannelInputModel model, MishMashDbContext context);

        FollowedChannelsViewModel GetFollowedChannels(string username, MishMashDbContext context);

        SuggestedChannelsViewModel GetSuggestedChannels(string username, MishMashDbContext context);

        SeeOtherChannelsViewModel GetSeeOtherChannels(string username, MishMashDbContext context);

        bool IsFollow(string username, int id, MishMashDbContext context);

        bool IsUnfollow(string username, int id, MishMashDbContext context);

        int Create(CreateChannelsInputModel model, MishMashDbContext context);
    }
}
