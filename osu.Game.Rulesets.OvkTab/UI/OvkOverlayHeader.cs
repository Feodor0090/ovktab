using osu.Framework.Graphics;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.UI.Components.Account;

namespace osu.Game.Rulesets.OvkTab.UI
{
    public class OvkOverlayHeader : TabControlOverlayHeader<OVKSections>
    {
        public OvkOverlayHeader()
        {
            AutoSizeAxes = Axes.Y;
        }

        protected override OverlayTitle CreateTitle() => new OvkTitle();

        protected override Drawable CreateTitleContent() => new HeaderProfileBadge();

        private class OvkTitle : OverlayTitle
        {
            public OvkTitle()
            {
                Title = "ovk tab";
                Description = "browse VK without exiting osu!";
                IconTexture = "Icons/Hexacons/contests";
            }
        }
    }
}
