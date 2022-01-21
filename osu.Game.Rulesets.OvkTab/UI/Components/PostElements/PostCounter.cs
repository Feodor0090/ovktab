using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public class PostCounter : RollingCounter<int>
    {
        public PostCounter()
        {
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;
            Margin = new MarginPadding() { Right = 10 };

        }
        protected override double RollingDuration => 1500;
        protected override Easing RollingEasing => Easing.Out;
        protected override OsuSpriteText CreateSpriteText()
        {
            return new OsuSpriteText()
            {
                Font = OsuFont.GetFont(size: 20, weight: FontWeight.Medium, fixedWidth: true),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        }
    }
}
