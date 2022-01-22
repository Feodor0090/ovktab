using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.API;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public class SendPopover : OsuPopover
    {
        readonly FillFlowContainer content;
        private readonly int ownerId;
        private readonly int postId;

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
        [Resolved] private DialogOverlay DialogOverlay { get; set; }
        [Resolved] private IOvkApiHub Api { get; set; }

        [BackgroundDependencyLoader(true)]
        async void load()
        {
            foreach (var x in await Api.GetDialogsList())
            {
                var u = x.Item1;
                if (u == null) u = new SimpleVkUser()
                {
                    name = x.Item2.Conversation.ChatSettings?.Title,
                    avatarUrl = x.Item2.Conversation.ChatSettings?.Photo?.Photo100.AbsoluteUri
                };
                content.Add(new DrawableActionableDialog(u, (int)x.Item2.Conversation.Peer.Id, Send));
            }
        }

        void Send(int peerId, string name) => DialogOverlay.Push(new SendDialog(name, async () =>
        {
            await Api.SendWall(peerId, ownerId, postId);
        }));
    }
}
