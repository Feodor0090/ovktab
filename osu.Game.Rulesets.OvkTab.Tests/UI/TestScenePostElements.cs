using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.OvkTab.UI.Components;
using osu.Framework.Graphics;
using osu.Framework.Bindables;
using osu.Game.Tests.Visual;
using osu.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Cursor;
using VkNet.Model.Attachments;

namespace osu.Game.Rulesets.OvkTab.Tests.UI
{
    public class TestScenePostElements : OsuTestScene
    {
        private FillFlowContainer headerContainer;
        private FillFlowContainer footerContainer;

        BindableInt likes = new BindableInt(0);
        BindableInt reposts = new BindableInt(0);
        BindableInt comments = new BindableInt(0);

        NowPlayingOverlay npo;

        [Cached]
        OverlayColourProvider ocp = new OverlayColourProvider(OverlayColourScheme.Blue);

        public TestScenePostElements()
        {
            Add(new PopoverContainer {
                RelativeSizeAxes = Axes.Both,
                Child = new GridContainer()
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Size = new(0.7f, 0.5f),
                    Content = new Drawable[][]
                    {
                        new Drawable[]
                        {
                            headerContainer = new FillFlowContainer()
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        },
                        new Drawable[]
                        {
                            footerContainer = new FillFlowContainer()
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    }
                }
            });
        }
        [BackgroundDependencyLoader]
        void load()
        {
            LoadComponentAsync(new NowPlayingOverlay(), o =>
            {
                o.Origin = o.Anchor = Anchor.TopLeft;
                Add(o);
                npo = o;
            });
        }
        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddStep("Show NPO", () => npo?.Show());
            AddStep("Add header", () =>
            {
                headerContainer.Child = new PostHeader(new OvkApiHub.SimpleUser()
                {
                    avatarUrl = "https://sun9-87.userapi.com/impg/zKRqPgUEstF259F3AT1x47pAAziAYSOPyarB6w/S5nHYGmZg9Y.jpg?size=200x201&quality=96&sign=fab58cebed5730066207d0445ebd14d1&type=album",
                    id = 0,
                    name = "Выф фумович",
                }, DateTime.Now);
            });
            AddStep("Add footer", () =>
            {
                var f = new PostFooter((Post)null);
                footerContainer.Child = f;
                f.likes.BindTo(likes);
                f.reposts.BindTo(reposts);
                f.comments.BindTo(comments);
            });
            AddSliderStep("Likes", 0, 12960, 30, value => likes.Value = value);
            AddSliderStep("Comments", 0, 12960, 30, value => comments.Value = value);
            AddSliderStep("Reposts", 0, 12960, 30, value => reposts.Value = value);
        }
    }
}
