using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public class PostActionButton : OsuButton, IHasPopover
    {
        private readonly IconUsage icon;
        private OsuColour colour;
        private bool done;
        private readonly Func<Popover> popover;
        private readonly bool isPink;


        public bool Checked
        {
            get => done;
            set
            {
                done = value;
                if (done)
                {
                    BackgroundColour = colour.Lime;
                    Triangles.ColourDark = colour.Lime0;
                    Triangles.ColourLight = colour.Lime3;
                }
                else
                {
                    BackgroundColour = isPink ? colour.PinkDark : colour.BlueDark;
                    Triangles.ColourDark = isPink ? colour.PinkDarker : colour.BlueDarker;
                    Triangles.ColourLight = isPink ? colour.Pink : colour.Blue;
                }
            }
        }

        public PostActionButton(IconUsage icon, bool pink, bool done, Action action, Func<Popover> popover = null)
        {
            Action = action;
            this.icon = icon;
            isPink = pink;
            this.done = done;
            Size = new(30, 30);
            Anchor = Origin = Anchor.CentreLeft;
            this.popover = popover;
        }

        protected Triangles Triangles { get; private set; }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            colour = colours;
            Add(Triangles = new SmallTriangles());
            Add(new SpriteIcon()
            {
                Icon = icon,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new(18),
            });
            Checked = done;
        }

        public Popover GetPopover()
        {
            if (popover == null) return null;
            return popover();
        }

        private class SmallTriangles : Triangles
        {
            protected override float SpawnRatio => 2f;
            public SmallTriangles()
            {
                RelativeSizeAxes = Axes.Both;
                TriangleScale = 0.5f;
            }
        }
    }
}
