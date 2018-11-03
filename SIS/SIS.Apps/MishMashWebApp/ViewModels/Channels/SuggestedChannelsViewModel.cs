using System.Collections.Generic;

namespace MishMashWebApp.ViewModels.Channels
{
    public class SuggestedChannelsViewModel
    {
        public virtual ICollection<BaseChannelViewModel> SuggestedChannels { get; set; } = new List<BaseChannelViewModel>();
    }
}
