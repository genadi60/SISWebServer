using System.Collections.Generic;
using MishMashWebApp.ViewModels.Channels;

namespace MishMashWebApp.ViewModels.Home
{
    public class LoggedInIndexViewModel
    {
        public string Role { get; set; }

        public virtual ICollection<BaseChannelViewModel> YourChannels { get; set; } = new List<BaseChannelViewModel>();

        public virtual ICollection<BaseChannelViewModel> SuggestedChannels { get; set; } = new List<BaseChannelViewModel>();
              
        public virtual ICollection<BaseChannelViewModel> SeeOtherChannels { get; set; } = new List<BaseChannelViewModel>();
    }
}
