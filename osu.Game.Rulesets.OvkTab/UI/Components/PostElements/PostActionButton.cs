using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public partial class PostActionButton : OsuButton, IHasPopover, IHasTooltip
    {
        [Resolved]
        private OsuColour colours { get; set; } = null!;

        public Func<Popover>? popover;
        private readonly bool isPink;

        public readonly BindableBool state = new();

        public PostActionButton(IconUsage icon, bool pink)
        {
            isPink = pink;
            Size = new(30, 30);
            Anchor = Origin = Anchor.CentreLeft;

            Add(Triangles = new SmallTriangles());
            Add(new SpriteIcon
            {
                Icon = icon,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new(18),
            });
        }

        protected Triangles Triangles { get; }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            state.BindValueChanged(e =>
            {
                if (e.NewValue)
                {
                    BackgroundColour = colours.Lime;
                    Triangles.ColourDark = colours.Lime0;
                    Triangles.ColourLight = colours.Lime3;
                }
                else
                {
                    BackgroundColour = isPink ? colours.PinkDark : colours.BlueDark;
                    Triangles.ColourDark = isPink ? colours.PinkDarker : colours.BlueDarker;
                    Triangles.ColourLight = isPink ? colours.Pink : colours.Blue;
                }
            }, true);
        }

        public Popover GetPopover()
        {
            return popover?.Invoke()!;
        }

        private partial class SmallTriangles : Triangles
        {
            protected override float SpawnRatio => 2f;

            public SmallTriangles()
            {
                RelativeSizeAxes = Axes.Both;
                TriangleScale = 0.5f;
            }
        }

        public LocalisableString TooltipText { get; set; }
    }
}
