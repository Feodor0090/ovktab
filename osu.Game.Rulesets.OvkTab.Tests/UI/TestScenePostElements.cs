using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.Tests.Stubs;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;
using osu.Game.Tests.Visual;
using System;

namespace osu.Game.Rulesets.OvkTab.Tests.UI
{
    public partial class TestScenePostElements : OsuTestScene
    {
        private readonly FillFlowContainer headerContainer;
        private readonly FillFlowContainer footerContainer;
        readonly BindableInt likes = new();
        readonly BindableInt reposts = new();
        readonly BindableInt comments = new();

        NowPlayingOverlay npo;

        [Cached]
        public readonly OverlayColourProvider ocp = new(OverlayColourScheme.Blue);

        [Cached]
        public readonly DialogOverlay dialogOverlay = new();

        [Cached(typeof(IOvkApiHub))]
        public readonly IOvkApiHub apiHub = new OvkApiHubStub();

        [Cached]
        public readonly PopoverContainer pc;

        public TestScenePostElements()
        {
            Add(pc = new PopoverContainer
            {
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
                o.Origin = o.Anchor = Anchor.TopRight;
                Add(o);
                npo = o;
            });
            Add(dialogOverlay);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            apiHub.LoggedUser.Value = OvkApiHubStub.Maho;
            AddStep("Show NPO", () => npo.Show());
            AddStep("Add header", () =>
            {
                headerContainer.Child = new PostHeader(new SimpleVkUser()
                {
                    avatarUrl = "https://sun9-87.userapi.com/impg/zKRqPgUEstF259F3AT1x47pAAziAYSOPyarB6w/S5nHYGmZg9Y.jpg?size=200x201&quality=96&sign=fab58cebed5730066207d0445ebd14d1&type=album",
                    id = 727,
                    name = "Выф Mohovich",
                }, new DateTime(2022, 1, 1, 7, 27, 27));
            });
            AddStep("Add footer", () =>
            {
                var f = new PostFooter(292, 1296000);
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
