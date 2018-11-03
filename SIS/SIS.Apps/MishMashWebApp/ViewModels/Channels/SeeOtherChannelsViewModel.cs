using System.Collections.Generic;

namespace MishMashWebApp.ViewModels.Channels
{
    public class SeeOtherChannelsViewModel
    {
        public virtual ICollection<BaseChannelViewModel> SeeOtherChannels { get; set; } = new List<BaseChannelViewModel>();
    }
}
