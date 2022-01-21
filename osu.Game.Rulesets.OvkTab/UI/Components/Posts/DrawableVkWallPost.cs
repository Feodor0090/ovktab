using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;
using System;
using VkNet.Model.Attachments;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Posts
{
    public class DrawableVkWallPost : FillFlowContainer
    {
        Post post;
        SimpleVkUser author;
        public DrawableVkWallPost(Post post, SimpleVkUser author)
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
        void load(OsuGame game, LargeTextureStore lts)
        {
            Children = new Drawable[]
            {
                new PostHeader(author, post.Date??DateTime.UtcNow),
                new TextFlowContainer()
                {
                    Text = post.Text,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new() { Horizontal = 5 }
                }
            };
            AddRange(DrawableVkPost.parseAttachments(post.Attachments, Dependencies));
            // footer
            Add(new PostFooter(post));
        }
    }
}
