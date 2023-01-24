using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;
using System.Collections.Generic;
using System.Linq;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Comments
{
    public partial class DrawableVkComment : DrawableVkMessage
    {
        private readonly Comment comm;
        private readonly List<CommentsLevel> replies;
        private PostActionButton likeButton;
        private readonly Bindable<int> likes = new();

        private readonly bool canPost;
        private readonly bool canLike;

        [Resolved]
        private CommentsPopover comms { get; set; }

        [Resolved]
        private IOvkApiHub api { get; set; }

        public DrawableVkComment(CommentsLevel level, bool canPost, bool canLike)
            : base(level.user)
        {
            comm = level.comment;
            replies = level.replies;
            this.canPost = canPost;
            this.canLike = canLike;
        }

        [BackgroundDependencyLoader(true)]
        private void load()
        {
            AddContent(comm.Text, comm.Attachments, comm.Date ?? System.DateTime.Now);

            if (canLike)
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
                        likeButton = new LikeButton()
                        {
                            state = { Value = comm.Likes.UserLikes },
                            Action = async () =>
                            {
                                if (likeButton.state.Value) return;

                                likeButton.state.Value = true;
                                likes.Value++;
                                var l = await api.LikeComment(comm);

                                if (l.HasValue)
                                {
                                    likes.Value = (int)l.Value;
                                }
                                else
                                {
                                    likeButton.state.Value = false;
                                }
                            }
                        },
                        new PostCounter(likes),
                    }
                });
            }

            if (canPost)
            {
                var f = new OsuTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    PlaceholderText = "type your reply",
                };
                f.OnCommit += F_OnCommit;
                content.Add(f);
            }

            content.AddRange(replies.Select(x => new DrawableVkComment(x, canPost, canLike)));
        }

        private void F_OnCommit(Framework.Graphics.UserInterface.TextBox sender, bool newText)
        {
            comms.SendComment(sender, sender.Text, (int)comm.Id, content);
        }

        public struct CommentsLevel
        {
            public int id;
            public SimpleVkUser? user;
            public Comment comment;
            public List<CommentsLevel> replies;
        }

        public static CommentsLevel[] BuildTree(IEnumerable<Comment> source, SimpleVkUser[] users)
        {
            return source.Select(x => new CommentsLevel
            {
                id = (int)x.Id,
                user = users.FirstOrDefault(u => u.id == x.FromId),
                comment = x,
                replies = x.Thread?.Items.Select(y => new CommentsLevel
                {
                    id = (int)y.Id,
                    user = users.FirstOrDefault(u => u.id == y.FromId),
                    comment = y,
                    replies = new List<CommentsLevel>(),
                }).ToList() ?? new List<CommentsLevel>()
            }).ToArray();
        }
    }
}
