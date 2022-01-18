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

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class PostFooter : FillFlowContainer
    {
        private readonly int ownerId, postId;
        public readonly BindableInt likes = new BindableInt(0);
        public readonly BindableInt comments = new BindableInt(0);
        public readonly BindableInt reposts = new BindableInt(0);
        private PostActionButton likeButton;
        internal PostActionButton commentsButton;
        private PostActionButton repostButton;
        private PostActionButton sendButton;
        private PostActionButton faveButton;

        [Cached]
        internal PostFooter footer;

        [Resolved(canBeNull: true)] private OvkApiHub OvkApiHub { get; set; }
        [Resolved(canBeNull: true)] private OvkOverlay OvkOverlay { get; set; }
        [Resolved(canBeNull: true)] private NotificationOverlay nofs { get; set; }
        [Resolved(canBeNull: true)] private DialogOverlay dialogOverlay { get; set; }
        [Resolved(canBeNull: true)] private OsuGame osuGame { get; set; }
        [Resolved(canBeNull: true)] private PopoverContainer popoverContainer { get; set; }

        public PostFooter(NewsItem post)
        {
            ownerId = (int)(post?.SourceId ?? 0);
            postId = (int)(post?.PostId ?? 0);
            footer = this;

            likes.Value = post?.Likes?.Count ?? 0;
            comments.Value = post?.Comments?.Count ?? 0;
            reposts.Value = post?.Reposts?.Count ?? 0;

            Initialize(post?.Likes, post?.Reposts);
        }

        public PostFooter(Post post)
        {
            ownerId = (int)(post?.OwnerId ?? 0);
            postId = (int)(post?.Id ?? 0);
            footer = this;

            likes.Value = post?.Likes?.Count ?? 0;
            comments.Value = post?.Comments?.Count ?? 0;
            reposts.Value = post?.Reposts?.Count ?? 0;

            Initialize(post?.Likes, post?.Reposts);
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
            Children = new Drawable[]
            {
                likeButton = new PostActionButton(FontAwesome.Regular.Heart, true, likesInfo?.UserLikes??false, LikePost),
                new PostCounter()
                {
                    Current = likes
                },
                commentsButton = new PostActionButton(FontAwesome.Regular.Comment, false, false, OpenComments, ()=>new CommentsPopover(ownerId, postId, this, popoverContainer?.DrawSize ?? new(600))),
                new PostCounter()
                {
                    Current = comments
                },
                repostButton = new PostActionButton(FontAwesome.Solid.Bullhorn, false, repostsInfo?.UserReposted??false, (likesInfo?.CanPublish != false)?Repost:null),
                new PostCounter()
                {
                    Current = reposts
                },
                sendButton = new PostActionButton(FontAwesome.Regular.PaperPlane, false, false, () => {
                    sendButton.ShowPopover();
                }, ()=>new SendPopover(ownerId, postId)),
                faveButton = new PostActionButton(FontAwesome.Regular.Star, false, false, Fave),
                new PostActionButton(FontAwesome.Solid.Link, false, false, () =>
                {
                    osuGame?.HandleLink(new LinkDetails(LinkAction.External, $"https://vk.com/wall{ownerId}_{postId}"));
                }),
            };
        }

        async void LikePost()
        {
            if (postId == 0 || OvkApiHub == null) return;
            OvkOverlay?.newsLoading.Show();
            long? newCount = await OvkApiHub.LikePost(ownerId, postId, !likeButton.Checked);
            if (newCount.HasValue)
            {
                likeButton.Checked = !likeButton.Checked;
                likes.Value = (int)newCount.Value;
            }
            else
                nofs?.Post(new SimpleErrorNotification()
                {
                    Text = "Failed to like the post"
                });
            OvkOverlay.newsLoading.Hide();
        }

        void OpenComments()
        {
            commentsButton.ShowPopover();
        }

        void Repost()
        {
            if (postId == 0 || OvkApiHub == null) return;
            if (repostButton.Checked) return;

            RepostDialog dialog = new RepostDialog(ownerId, postId, OvkApiHub, OvkOverlay?.newsLoading, x =>
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
            dialogOverlay?.Push(dialog);
        }

        async void Fave()
        {
            if (postId == 0 || OvkApiHub == null) return;
            if (faveButton.Checked) return;
            OvkOverlay?.newsLoading.Show();
            bool ok = await OvkApiHub?.AddToBookmarks(ownerId, postId);
            if (ok)
            {
                faveButton.Checked = true;
            }
            else
            {
                nofs?.Post(new SimpleErrorNotification()
                {
                    Text = "Failed to fave the post"
                });
            }
            OvkOverlay?.newsLoading.Hide();
        }
    }
}
