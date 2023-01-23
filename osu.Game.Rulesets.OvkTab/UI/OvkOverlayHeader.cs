using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.UI.Components.Account;

namespace osu.Game.Rulesets.OvkTab.UI
{
    public partial class OvkOverlayHeader : TabControlOverlayHeader<OvkSections>
    {
        public OvkOverlayHeader()
        {
            AutoSizeAxes = Axes.Y;
        }

        protected override OverlayTitle CreateTitle() => new OvkTitle();

        private partial class OvkTitle : OverlayTitle
        {
            public OvkTitle()
            {
                Title = "ovk tab";
                Description = "browse VK without exiting osu!";
                IconTexture = "Icons/Hexacons/contests";
            }

            protected override void LoadComplete()
            {
                (Parent as Container<Drawable>)!.Add(new HeaderProfileBadge
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                });
            }
        }
    }
}
