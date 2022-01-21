using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.Posts;
using System.Linq;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class WallPopover : OsuPopover
    {
        readonly int ownerId;
        FillFlowContainer content;
        LoadingLayer ll;

        [Cached]
        PopoverContainer pc;
        public WallPopover(int id, osuTK.Vector2 parentSize)
        {
            ownerId = id;
            Size = parentSize * 0.95f;
            Content.AutoSizeAxes = Axes.None;
            Content.RelativeSizeAxes = Axes.Both;
            Content.Children = new Drawable[]
            {
                pc = new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new OverlayScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = content = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y
                        }
                    }
                },
                ll = new LoadingLayer()
            };
            ll.Show();
        }

        [BackgroundDependencyLoader]
        async void load(OvkApiHub api)
        {
            var r = await api.LoadWall(ownerId);
            var posts = r.Select(x => new DrawableVkWallPost(x.Item1, x.Item2)).ToArray();
            Schedule(() =>
            {
                content.AddRange(posts);
                ll.Hide();
            });
        }
    }
}
