using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Game.Tests.Visual;
using osuTK.Graphics;
using osu.Game.Rulesets.OvkTab.UI;
using osu.Game.Overlays;

namespace osu.Game.Rulesets.OvkTab.Tests
{
    public partial class TestSceneStandaloneOvkOverlay : OsuTestScene
    {
        [Cached]
        public readonly DialogOverlay dialogOverlay = new();

        [Cached(typeof(INotificationOverlay))]
        public readonly NotificationOverlay notifOverlay = new();

        [Cached]
        public readonly OvkOverlay ovkOverlay = new(null);

        [BackgroundDependencyLoader]
        private void load(GameHost host, OsuGameBase gameBase)
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                },
            };
            Add(notifOverlay);
            Add(dialogOverlay);
            Add(ovkOverlay);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddStep("Show", () => { ovkOverlay.Show(); });
        }
    }
}
