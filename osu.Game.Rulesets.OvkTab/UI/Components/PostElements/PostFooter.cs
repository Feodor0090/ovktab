using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Extensions;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Framework.Graphics.Sprites;
using VkNet.Model;
using osu.Game.Online.Chat;
using VkNet.Model.Attachments;
using osu.Framework.Graphics.Cursor;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.Comments;
using System;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public class PostFooter : FillFlowContainer
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

        [Resolved] private IOvkApiHub OvkApiHub { get; set; }
        [Resolved(canBeNull: true)] private OvkOverlay OvkOverlay { get; set; }
        [Resolved(canBeNull: true)] private NotificationOverlay Nofs { get; set; }
        [Resolved] private DialogOverlay DialogOverlay { get; set; }
        [Resolved(canBeNull: true)] private OsuGame OsuGame { get; set; }
        [Resolved(canBeNull: true)] private PopoverContainer PopoverContainer { get; set; }

        public PostFooter(NewsItem post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            ownerId = (int)post.SourceId;
            postId = (int)post.PostId;
            footer = this;

            likes.Value = post.Likes?.Count ?? 0;
            comments.Value = post.Comments?.Count ?? 0;
            reposts.Value = post.Reposts?.Count ?? 0;

            Initialize(post.Likes, post.Reposts);
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

            Initialize(post.Likes, post.Reposts);
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

            Initialize(post.Likes, post.Reposts);
        }

        public PostFooter()
        {
            ownerId = 0;
            postId = 0;
            footer = this;

            Initialize(null, null);
        }
        public PostFooter(int owner, int id)
        {
            ownerId = owner;
            postId = id;
            footer = this;

            Initialize(null, null);
        }

        private void Initialize(Likes likesInfo, Reposts repostsInfo)
        {
            Direction = FillDirection.Horizontal;
            RelativeSizeAxes = Axes.X;
            Height = 40;
            Spacing = new(5);
            Padding = new MarginPadding()
            {
                Left = 5
            };
            bool userLikes = likesInfo?.UserLikes ?? false;
            Children = new Drawable[]
            {
                likeButton = new PostActionButton(FontAwesome.Regular.Heart, true, userLikes, Like),
                new PostCounter(likes),
                commentsButton = new PostActionButton(FontAwesome.Regular.Comment, false, false, ()=>{
                    commentsButton.ShowPopover();
                }, ()=>new CommentsPopover(ownerId, postId, this, PopoverContainer?.DrawSize ?? new(600))),
                new PostCounter(comments),
                repostButton = new PostActionButton(FontAwesome.Solid.Bullhorn, false, repostsInfo?.UserReposted??false, likesInfo?.CanPublish != false?Repost:null),
                new PostCounter(reposts),
                sendButton = new PostActionButton(FontAwesome.Regular.PaperPlane, false, false,
                    ()=>{
                        sendButton.ShowPopover(); 
                    }, 
                    ()=>new SendPopover(ownerId, postId)),
                faveButton = new PostActionButton(FontAwesome.Regular.Star, false, false, Fave),
                new PostActionButton(FontAwesome.Solid.Link, false, false, () =>
                {
                    OsuGame?.HandleLink(new LinkDetails(LinkAction.External, $"https://vk.com/wall{ownerId}_{postId}"));
                }),
            };
        }

        async void Like()
        {
            if (postId == 0) return;
            OvkOverlay?.newsLoading.Show();
            long? newCount = await OvkApiHub.LikePost(ownerId, postId, !likeButton.Checked);
            if (newCount.HasValue)
            {
                likeButton.Checked = !likeButton.Checked;
                likes.Value = (int)newCount.Value;
            }
            else
                Nofs?.Post(new SimpleErrorNotification()
                {
                    Text = "Failed to like the post"
                });
            OvkOverlay?.newsLoading.Hide();
        }

        void Repost()
        {
            if (postId == 0 || repostButton.Checked) return;

            RepostDialog dialog = new(ownerId, postId, OvkApiHub, OvkOverlay?.newsLoading, x =>
            {
                OvkOverlay?.newsLoading.Hide();
                if (x.HasValue)
                {
                    repostButton.Checked = true;
                    likeButton.Checked = true;
                    if (x.Value.Item1.HasValue) likes.Value = x.Value.Item1.Value;
                    if (x.Value.Item2.HasValue) reposts.Value = x.Value.Item2.Value;
                }
            });
            DialogOverlay.Push(dialog);
        }

        async void Fave()
        {
            if (postId == 0 || faveButton.Checked) return;
            OvkOverlay?.newsLoading.Show();
            bool ok = await OvkApiHub.AddToBookmarks(ownerId, postId);
            if (ok)
            {
                faveButton.Checked = true;
            }
            else
            {
                Nofs?.Post(new SimpleErrorNotification()
                {
                    Text = "Failed to fave the post. Is it favourite already?"
                });
            }
            OvkOverlay?.newsLoading.Hide();
        }
    }
}
