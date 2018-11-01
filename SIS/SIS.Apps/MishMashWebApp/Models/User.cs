using MishMashWebApp.Models.Enums;
using System.Collections.Generic;

namespace MishMashWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public virtual ICollection<UserChannel> FollowedChannels { get; set; } = new HashSet<UserChannel>();

        public Role Role { get; set; }


    }
}
