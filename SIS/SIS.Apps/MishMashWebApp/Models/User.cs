namespace MishMashWebApp.Models
{
    using System.Collections.Generic;

    using Enums;

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
