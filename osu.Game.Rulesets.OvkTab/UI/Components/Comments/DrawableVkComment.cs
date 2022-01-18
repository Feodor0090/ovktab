using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Graphics.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model;
using static osu.Game.Rulesets.OvkTab.OvkApiHub;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class DrawableVkComment : DrawableVkMessage
    {
        Comment comm;
        List<CommentsLevel> replies;
        private PostActionButton likeButton;
        private Bindable<int> likes = new Bindable<int>();

        bool CanPost { get; set; }
        bool CanLike { get; set; }

        [Resolved]
        private CommentsPopover comms { get; set; }
        [Resolved]
        private OvkApiHub api { get; set; }

        public DrawableVkComment(CommentsLevel level, bool canPost, bool canLike) : base(level.user)
        {
            comm = level.comment;
            replies = level.replies;
            CanPost = canPost;
            CanLike = canLike;
        }

        [BackgroundDependencyLoader(true)]
        void load(OsuGame game, LargeTextureStore lts)
        {
            AddContent(comm.Text, comm.Attachments, game, lts);
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
                        new PostCounter()
                        {
                            Current = likes
                        },
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

            // old method to build full tree
            List<Comment> replies = new List<Comment>();
            List<Comment> roots = new List<Comment>();
            foreach (Comment comment in source)
            {
                if (comment.ReplyToComment.HasValue)
                {
                    replies.Add(comment);
                }
                else
                {
                    roots.Add(comment);
                }
            }
            var result = roots.Select(x => new CommentsLevel
            {
                id = (int)x.Id,
                user = users.Where(u => u.id == x.FromId).First(),
                comment = x,
                replies = new(),
            }).ToArray();

            void PushReply(CommentsLevel level)
            {
                for (int i = replies.Count - 1; i >= 0; i--)
                {
                    if (replies[i].ReplyToComment == level.id)
                    {
                        level.replies.Add(new CommentsLevel
                        {
                            id = (int)replies[i].Id,
                            user = users.Where(u => u.id == replies[i].FromId).First(),
                            comment = replies[i],
                            replies = new(),
                        });
                        replies.RemoveAt(i);
                    }
                }
                for (int i = 0; i < level.replies.Count; i++)
                {
                    PushReply(level.replies[i]);
                }
            }

            while (replies.Count > 0)
            {
                for (int i = 0; i < result.Length; i++)
                    PushReply(result[i]);
            }

            return result;
        }
    }
}
