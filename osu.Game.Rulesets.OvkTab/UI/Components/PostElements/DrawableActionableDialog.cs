using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using System;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class DrawableActionableDialog : OsuClickableContainer
    {
        DrawableVkAvatar avatar;
        readonly Container hoverBox;
        public DrawableActionableDialog(SimpleVkUser user, int peerId, Action<int, string> action)
        {
            Height = 50;
            RelativeSizeAxes = Axes.X;
            Padding = new MarginPadding { Right = 15 };
            var font = OsuFont.GetFont(size: 20);
            Children = new Drawable[]
            {
                hoverBox = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 10,
                    Masking = true
                },
                new OsuSpriteText()
                {
                    Text = user?.name ?? "Unknown chat",
                    Position = new(65, 25),
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.X,
                    Font = font,
                    Truncate = true
                },
            };
            avatar = new DrawableVkAvatar(user)
            {
                Position = new(25),
            };
            Action = () => action(peerId, user.name);
        }
        [BackgroundDependencyLoader]
        void load(OsuColour c)
        {
            hoverBox.Add(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = c.BlueDarker
            });
            hoverBox.Add(new Triangles() { ColourDark = c.Blue3, ColourLight = c.BlueDark, RelativeSizeAxes = Axes.Both });
            hoverBox.Hide();
            LoadComponentAsync(avatar, x => { Add(x); avatar = x; });
        }

        protected override bool OnHover(HoverEvent e)
        {
            hoverBox.FadeIn(250);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hoverBox.FadeOut(500);
            base.OnHoverLost(e);
        }
    }
}
