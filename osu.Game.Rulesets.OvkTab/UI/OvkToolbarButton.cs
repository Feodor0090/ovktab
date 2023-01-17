using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Overlays.Toolbar;

namespace osu.Game.Rulesets.OvkTab.UI
{
    public partial class OvkToolbarButton : ToolbarOverlayToggleButton
    {
        protected override Anchor TooltipAnchor => Anchor.TopRight;

        public OvkToolbarButton()
        {
            //Hotkey = GlobalAction.ToggleChat;
        }

        [BackgroundDependencyLoader(true)]
        private void load(OvkOverlay overlay)
        {
            StateContainer = overlay;
        }
    }
}
