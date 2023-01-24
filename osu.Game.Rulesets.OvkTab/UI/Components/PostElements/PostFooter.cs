using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Extensions;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Framework.Graphics.Sprites;
using VkNet.Model;
using VkNet.Model.Attachments;
using osu.Framework.Graphics.Cursor;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.Comments;
using System;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public partial class PostFooter : FillFlowContainer
    {
        private readonly int ownerId, postId;
        public readonly BindableInt likes = new();
        public readonly BindableInt comments = new();
        public readonly BindableInt reposts = new();
        private PostActionButton likeButton;
        internal PostActionButton commentsButton;
        private PostActionButton repostButton;
        private PostActionButton sendButton;
        private PostActionButton faveButton;

        [Cached]
        private PostFooter footer;

        [Resolved]
        private IOvkApiHub ovkApiHub { get; set; } = null!;

        [Resolved]
        private OvkOverlay? ovkOverlay { get; set; }

        [Resolved]
        private INotificationOverlay? nofs { get; set; }

        [Resolved]
        private IDialogOverlay dialogOverlay { get; set; } = null!;

        [Resolved]
        private PopoverContainer? popoverContainer { get; set; }

        public PostFooter(NewsItem post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            ownerId = (int)post.SourceId;
            postId = (int)post.PostId;
            footer = this;

            likes.Value = post.Likes?.Count ?? 0;
            comments.Value = post.Comments?.Count ?? 0;
            reposts.Value = post.Reposts?.Count ?? 0;

            initialize(post.Likes, post.Reposts);
        }

        public PostFooter(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            ownerId = (int)post.OwnerId;
            postId = (int)post.Id;
            footer = this;

            likes.Value = post.Likes?.Count ?? 0;
            comments.Value = post.Comments?.Count ?? 0;
            reposts.Value = post.Reposts?.Count ?? 0;

            initialize(post.Likes, post.Reposts);
        }

        public PostFooter(Wall post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            ownerId = (int)post.OwnerId;
            postId = (int)post.Id;
            footer = this;

            likes.Value = post.Likes?.Count ?? 0;
            comments.Value = post.Comments?.Count ?? 0;
            reposts.Value = post.Reposts?.Count ?? 0;

            initialize(post.Likes, post.Reposts);
        }

        public PostFooter()
        {
            ownerId = 0;
            postId = 0;
            footer = this;

            initialize(null, null);
        }

        public PostFooter(int owner, int id)
        {
            ownerId = owner;
            postId = id;
            footer = this;

            initialize(null, null);
        }

        private void initialize(Likes? likesInfo, Reposts? repostsInfo)
        {
            Direction = FillDirection.Horizontal;
            RelativeSizeAxes = Axes.X;
            Height = 40;
            Spacing = new(5);
            Padding = new MarginPadding { Left = 5 };
            bool userLikes = likesInfo?.UserLikes ?? false;
            Children = new Drawable[]
            {
                likeButton = new LikeButton
                {
                    state = { Value = userLikes },
                    Action = like
                },
                new PostCounter(likes),
                commentsButton = new PostActionButton(FontAwesome.Regular.Comment, false)
                {
                    TooltipText = "open comments",
                    Action = () => commentsButton.ShowPopover(),
                    popover = () => new CommentsPopover(ownerId, postId, this, popoverContainer?.DrawSize ?? new(600))
                },
                new PostCounter(comments),
                repostButton = new PostActionButton(FontAwesome.Solid.Bullhorn, false)
                {
                    TooltipText = "repost to your page",
                    state = { Value = repostsInfo?.UserReposted ?? false },
                    Action = likesInfo?.CanPublish != false ? repost : null
                },
                new PostCounter(reposts),
                sendButton = new PostActionButton(FontAwesome.Regular.PaperPlane, false)
                {
                    TooltipText = "send in messages",
                    Action = () => sendButton.ShowPopover(),
                    popover = () => new SendPopover(ownerId, postId)
                },
                faveButton = new PostActionButton(FontAwesome.Regular.Star, false) { TooltipText = "toogle bookmark", Action = fave },
                new ViewPostButton($"https://vk.com/wall{ownerId}_{postId}"),
            };
        }

        async void like()
        {
            if (postId == 0) return;
            ovkOverlay?.newsLoading.Show();
            long? newCount = await ovkApiHub.LikePost(ownerId, postId, !likeButton.state.Value);

            if (newCount.HasValue)
            {
                likeButton.state.Toggle();
                likes.Value = (int)newCount.Value;
            }
            else
            {
                nofs?.Post(new SimpleErrorNotification
                {
                    Text = "Failed to like the post"
                });
            }

            ovkOverlay?.newsLoading.Hide();
        }

        void repost()
        {
            if (postId == 0 || repostButton.state.Value) return;

            RepostDialog dialog = new(ownerId, postId, ovkOverlay?.newsLoading, x =>
            {
                ovkOverlay?.newsLoading.Hide();

                if (x.HasValue)
                {
                    repostButton.state.Value = true;
                    likeButton.state.Value = true;
                    if (x.Value.Item1.HasValue) likes.Value = x.Value.Item1.Value;
                    if (x.Value.Item2.HasValue) reposts.Value = x.Value.Item2.Value;
                }
            });
            dialogOverlay.Push(dialog);
        }

        async void fave()
        {
            if (postId == 0 || faveButton.state.Value) return;

            ovkOverlay?.newsLoading.Show();
            bool ok = await ovkApiHub.AddToBookmarks(ownerId, postId);

            if (ok)
            {
                faveButton.state.Value = true;
            }
            else
            {
                nofs?.Post(new SimpleErrorNotification
                {
                    Text = "Failed to fave the post. Is it favourite already?"
                });
            }

            ovkOverlay?.newsLoading.Hide();
        }
    }
}
