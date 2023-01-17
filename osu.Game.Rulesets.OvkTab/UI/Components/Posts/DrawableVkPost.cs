using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using VkNet.Model;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;
using osu.Game.Rulesets.OvkTab.API;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Posts
{
    public partial class DrawableVkPost : FillFlowContainer
    {
        private readonly NewsItem post;
        private readonly SimpleVkUser author;

        public DrawableVkPost(NewsItem post, SimpleVkUser author)
        {
            this.post = post;
            this.author = author;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Spacing = new(10);
            Padding = new() { Horizontal = 10 };
            Margin = new() { Bottom = 25 };
        }

        [BackgroundDependencyLoader(true)]
        private void load()
        {
            Children = new Drawable[]
            {
                new PostHeader(author, post.Date ?? DateTime.UtcNow),
                new TextFlowContainer
                {
                    Text = post.Text,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Horizontal = 5 }
                }
            };
            AddRange(post.Attachments.ParseAttachments(Dependencies));
            // footer
            Add(new PostFooter(post));
        }
    }
}
