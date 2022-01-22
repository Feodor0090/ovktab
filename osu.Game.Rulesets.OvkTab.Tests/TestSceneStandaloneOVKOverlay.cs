using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Game.Tests.Visual;
using osuTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.OvkTab.UI;
using System.Threading.Tasks;
using osu.Game.Overlays;

namespace osu.Game.Rulesets.OvkTab.Tests
{
    public class TestSceneStandaloneOVKOverlay : OsuTestScene
    {

        [Cached]
        public readonly DialogOverlay dialogOverlay = new();
        [Cached]
        public readonly NotificationOverlay notifOverlay = new();
        [Cached]
        public readonly OvkOverlay ovkOverlay = new(null);
        [Cached]
        public readonly NowPlayingOverlay npo = new();

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
            Add(npo);
            Add(dialogOverlay);
            Add(ovkOverlay);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddStep("Run", () =>
            {
                ovkOverlay.Show();
            });
            AddStep("Show NPO", npo.Show);
        }
    }
}
