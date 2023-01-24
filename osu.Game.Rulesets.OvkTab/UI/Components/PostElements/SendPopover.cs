using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.API;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public partial class SendPopover : OsuPopover
    {
        private readonly FillFlowContainer content;
        private readonly int ownerId;
        private readonly int postId;

        [Resolved]
        private IDialogOverlay dialogOverlay { get; set; } = null!;

        [Resolved]
        private IOvkApiHub api { get; set; } = null!;

        public SendPopover(int ownerId, int postId)
        {
            this.ownerId = ownerId;
            this.postId = postId;
            Add(new OverlayScrollContainer
            {
                Size = new(300, 400),
                Child = content = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new(5)
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            foreach (var x in api.GetDialogsList().Result)
            {
                var u = x.Item1;
                if (u == null)
                    u = new SimpleVkUser
                    {
                        name = x.Item2.Conversation.ChatSettings?.Title ?? string.Empty,
                        avatarUrl = x.Item2.Conversation.ChatSettings?.Photo?.Photo100.AbsoluteUri
                    };
                content.Add(new DrawableActionableDialog(u, (int)x.Item2.Conversation.Peer.Id, send));
            }
        }

        private void send(int peerId, string name) => dialogOverlay.Push(new SendDialog(name, async () => { await api.SendWall(peerId, ownerId, postId); }));
    }
}
