using System.Collections.Generic;
using MishMashWebApp.Models.Enums;

namespace MishMashWebApp.ViewModels.Channels
{
    public class ChannelViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ChannelType Type { get; set; }

        public string Description { get; set; }

        public virtual ICollection<string> Tags { get; set; } = new List<string>();

        public string TagsAsString => string.Join(", ", Tags);

        public int FollowersCount { get; set; }
    }
}
