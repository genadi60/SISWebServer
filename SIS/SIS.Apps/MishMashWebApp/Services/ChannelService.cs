using System;
using System.Collections.Generic;
using System.Linq;
using MishMashWebApp.Data;
using MishMashWebApp.InputModels.Channels;
using MishMashWebApp.Models;
using MishMashWebApp.Models.Enums;
using MishMashWebApp.Services.Contracts;
using MishMashWebApp.ViewModels.Channels;

namespace MishMashWebApp.Services
{
    public class ChannelService : IChannelService
    {
        public ChannelViewModel GetChannelViewModel(ChannelInputModel model, MishMashDbContext context)
        {
            using (context)
            {
                var viewModel = context.Channels
                    .Where(c => c.Id == model.Id)
                    .Select(c => new ChannelViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = c.Type,
                        Description = c.Description,
                        Tags = c.Tags.Select(t => t.Tag.Name).ToList(),
                        FollowersCount = c.Followers.Count
                    })
                    .FirstOrDefault();

                return viewModel;
            }
        }

        public FollowedChannelsViewModel GetFollowedChannels(string username, MishMashDbContext context)
        {
            var viewModel = new FollowedChannelsViewModel();

            var followedChannels = context.Channels
                .Where(c => c.Followers.Any(f => f.User.Username.Equals(username)))
                .Select(c => new BaseChannelViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    FollowersCount = c.Followers.Count
                })
                .ToList();

            viewModel.FollowedChannels = followedChannels;

            return viewModel;
        }

        public SuggestedChannelsViewModel GetSuggestedChannels(string username, MishMashDbContext context)
        {
            var followedChannelsTag = context.Channels
                .Where(c => c.Followers.Any(f => f.User.Username.Equals(username)))
                .ToList();
            var followedChannelsTagIds = new List<int>();
            if (followedChannelsTag.Count > 0)
            {
                followedChannelsTagIds = followedChannelsTag
                    .SelectMany(c => c.Tags.Select(t => t.TagId))
                    .ToList();
            }

            var suggestedChannels = context.Channels
                .Where(c => c.Followers.Any(f => !f.User.Username.Equals(username) && c.Tags.Any(t => followedChannelsTagIds.Contains(t.TagId))))
                .Select(c => new BaseChannelViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    FollowersCount = c.Followers.Count,
                    ChannelTagsIds = c.Tags.Select(t => t.Id).ToList()
                })
                .ToList();
            var viewModel = new SuggestedChannelsViewModel
            {
                SuggestedChannels = suggestedChannels
            };

            return viewModel;
        }

        public SeeOtherChannelsViewModel GetSeeOtherChannels(string username, MishMashDbContext context)
        {
            using (context)
            {
                var followedChannelsTagIds = context.Channels
                    .Where(c => c.Followers.Any(f => f.User.Username.Equals(username)))
                    .SelectMany(c => c.Tags.Select(t => t.TagId))
                    .ToList();

                var suggestedChannelsTagIds = context.Channels
                    .Where(c => c.Tags.Any(t => followedChannelsTagIds.Contains(t.TagId)) && c.Followers.Any(f => !f.User.Username.Equals(username)))
                    .SelectMany(c => c.Tags.Select(t => t.TagId))
                    .ToList();

                var seeOtherChannelsTags = followedChannelsTagIds.Concat(suggestedChannelsTagIds).Distinct();

                var seeOtherChannels = context.Channels
                    .Where(c => c.Tags.Any(t => !seeOtherChannelsTags.Contains(t.TagId)))
                    .Select(c => new BaseChannelViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = c.Type,
                        FollowersCount = c.Followers.Count,
                        ChannelTagsIds = c.Tags.Select(t => t.Id).ToList()
                    })
                    .ToList();
                var viewModel = new SeeOtherChannelsViewModel
                {
                    SeeOtherChannels = seeOtherChannels
                };

                return viewModel;
            }
        }

        public bool IsFollow(string username, int id, MishMashDbContext context)
        {
            using (context)
            {
                var user = context.Users.FirstOrDefault(u => u.Username.Equals(username));

                if (!context.UserChannel.Any(uc => uc.UserId == user.Id && uc.ChannelId == id))
                {
                    context.UserChannel.Add(new UserChannel
                    {
                        UserId = user.Id,
                        ChannelId = id
                    });

                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public bool IsUnfollow(string username, int id, MishMashDbContext context)
        {
            using (context)
            {
                var user = context.Users.FirstOrDefault(u => u.Username.Equals(username));

                if (context.UserChannel.Any(uc => uc.UserId == user.Id && uc.ChannelId == id))
                {
                    var followedChannel = context.UserChannel.FirstOrDefault(c => c.ChannelId == id);

                    if (followedChannel != null)
                    {
                        context.UserChannel.Remove(followedChannel);
                        context.SaveChanges();
                        return true;
                    }
                }

                return false;
            }
        }

        public int Create(CreateChannelsInputModel model, MishMashDbContext context)
        {
            using (context)
            {
                if (!Enum.TryParse(model.Type, true, out ChannelType channelType))
                {
                    return 0;
                }

                var channel = new Channel
                {
                    Name = model.Name,
                    Description = model.Description,
                    Type = channelType
                };

                var tags = model.Tags.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var tagName in tags)
                {
                    var tag = context.Tags.FirstOrDefault(t => t.Name.Equals(tagName));
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        context.Tags.Add(tag);
                        context.SaveChanges();
                    }

                    channel.Tags.Add(new ChannelTag { TagId = tag.Id });
                }

                context.Channels.Add(channel);
                context.SaveChanges();

                return channel.Id;
            }
        }
    }
}
