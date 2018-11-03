using System.Collections.Generic;
using MishMashWebApp.Models;

namespace MishMashWebApp.ViewModels.Channels
{
    using Models.Enums;

    public class BaseChannelViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ChannelType Type { get; set; }

        public int FollowersCount { get; set; }

        public virtual ICollection<int> FollowersIds { get; set; } = new List<int>();

        public virtual ICollection<int> ChannelTagsIds { get; set; } = new List<int>();
    }
}
