using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;
using System.Collections.Generic;
using System.Linq;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Comments
{
    public class DrawableVkComment : DrawableVkMessage
    {
        readonly Comment comm;
        readonly List<CommentsLevel> replies;
        private PostActionButton likeButton;
        private readonly Bindable<int> likes = new();

        bool CanPost { get; set; }
        bool CanLike { get; set; }

        [Resolved]
        private CommentsPopover comms { get; set; }
        [Resolved]
        private IOvkApiHub api { get; set; }

        public DrawableVkComment(CommentsLevel level, bool canPost, bool canLike) : base(level.user)
        {
            comm = level.comment;
            replies = level.replies;
            CanPost = canPost;
            CanLike = canLike;
        }

        [BackgroundDependencyLoader(true)]
        void load()
        {
            AddContent(comm.Text, comm.Attachments);
            if (CanLike)
            {
                likes.Value = comm.Likes.Count;
                content.Add(new FillFlowContainer
                {
                    Height = 30,
                    RelativeSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                    Spacing = new(5),
                    Padding = new MarginPadding()
                    {
                        Left = 5
                    },
                    Children = new Drawable[]
                    {
                        likeButton = new PostActionButton(FontAwesome.Regular.Heart, true, comm.Likes.UserLikes, async ()=>
                        {
                            if(likeButton.Checked) return;
                            likeButton.Checked = true;
                            likes.Value++;
                            var l = await api.LikeComment(comm);
                            if(l.HasValue)
                            {
                                likes.Value = (int)l.Value;
                            }
                            else
                            {
                                likeButton.Checked = false;
                            }
                        }),
                        new PostCounter(likes),
                    }
                });
            }
            if (CanPost)
            {
                var f = new OsuTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    PlaceholderText = "type your reply",

                };
                f.OnCommit += F_OnCommit;
                content.Add(f);
            }
            content.AddRange(replies.Select(x => new DrawableVkComment(x, CanPost, CanLike)));
        }

        private void F_OnCommit(Framework.Graphics.UserInterface.TextBox sender, bool newText)
        {
            comms.SendComment(sender, sender.Text, (int)comm.Id, content);
            return;
        }

        public struct CommentsLevel
        {
            public int id;
            public SimpleVkUser user;
            public Comment comment;
            public List<CommentsLevel> replies;
        }

        public static CommentsLevel[] BuildTree(IEnumerable<Comment> source, SimpleVkUser[] users)
        {
            return source.Select(x => new CommentsLevel
            {
                id = (int)x.Id,
                user = users.Where(u => u.id == x.FromId).FirstOrDefault(),
                comment = x,
                replies = x.Thread?.Items.Select(y => new CommentsLevel
                {
                    id = (int)y.Id,
                    user = users.Where(u => u.id == y.FromId).FirstOrDefault(),
                    comment = y,
                    replies = new(),
                }).ToList() ?? new()
            }).ToArray();
        }
    }
}
