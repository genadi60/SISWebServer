using System.Collections.Generic;

namespace MishMashWebApp.ViewModels.Channels
{
    public class FollowedChannelsViewModel
    {
        public FollowedChannelsViewModel()
        {
            
        }
        public virtual ICollection<BaseChannelViewModel> FollowedChannels { get; set; } = new List<BaseChannelViewModel>();
    }
}
