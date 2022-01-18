using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Cursor;
using osu.Game.Overlays;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using osu.Game.Rulesets.OvkTab;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Framework.Graphics.Sprites;
using osu.Game.Online.Chat;
using osu.Game.Graphics;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.OvkTab.UI.Components
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
