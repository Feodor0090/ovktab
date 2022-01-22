using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.UserInterface;
using osuTK;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Misc
{
    public class IconTrianglesButton : OsuButton
    {
        public IconUsage icon;
        public Vector2 iconSize;
        protected Triangles Triangles { get; private set; }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Add(Triangles = new Triangles
            {
                RelativeSizeAxes = Axes.Both,
                ColourDark = colours.BlueDarker,
                ColourLight = colours.Blue,
            });
            Add(new SpriteIcon
            {
                Icon = icon,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = iconSize,
            });
        }
    }
}
