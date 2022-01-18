using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using System.Collections.Generic;
using VkNet.Model.Attachments;
using static osu.Game.Rulesets.OvkTab.OvkApiHub;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public abstract class DrawableVkMessage : Container
    {
        protected SimpleVkUser user;
        public FillFlowContainer content;
        public DrawableVkMessage(SimpleVkUser author)
        {
            user = author;

            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            var font = OsuFont.GetFont(size: 20);

            if (user != null) Add(new OsuSpriteText
            {
                Text = user.name,
                Position = new(60, 0),
                Font = font,
            });

            Add(new Container
            {
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Padding = new MarginPadding { Right = 5, Bottom = 5, Left = 55, Top = 20 },
                Child = new Container
                {
                    Masking = true,
                    CornerRadius = 10,
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0.3f,
                            Colour = Colour4.Black
                        },
                        content = new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Y,
                            RelativeSizeAxes = Axes.X,
                            Padding = new MarginPadding(5),
                            Spacing = new(0, 10),
                        }
                    }
                }
            });
        }

        protected void AddContent(string text, IEnumerable<Attachment> atts)
        {
            content.Add(new TextFlowContainer(x => x.Font = OsuFont.GetFont(size: 18))
            {
                Text = ClearText(text),
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new() { Horizontal = 5 },
            });
            content.AddRange(DrawableVkPost.parseAttachments(atts, Dependencies));

            LoadComponentAsync(new DrawableVkAvatar(user)
            {
                Position = new(30, 25),
            }, Add);
        }
    }
}
