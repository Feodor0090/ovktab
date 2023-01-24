using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;
using System;
using VkNet.Model.Attachments;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Posts
{
    public sealed partial class DrawableVkWall : FillFlowContainer
    {
        private readonly Wall post;
        private readonly SimpleVkUser author;

        public DrawableVkWall(Wall post, SimpleVkUser author)
        {
            this.post = post;
            this.author = author;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Spacing = new(10);
            Padding = new() { Horizontal = 10 };
            Margin = new() { Bottom = 25 };
        }

        [BackgroundDependencyLoader]
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
            Add(new PostFooter(post));
        }
    }
}
