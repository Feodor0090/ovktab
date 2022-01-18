using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using VkNet.Model;
using osu.Game.Online.Chat;
using osu.Game.Graphics.UserInterfaceV2;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class SendPopover : OsuPopover
    {
        FillFlowContainer content;
        int ownerId, postId;
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
        [Resolved] private DialogOverlay dialogOverlay { get; set; }
        [Resolved] private OvkApiHub api { get; set; }

        [BackgroundDependencyLoader(true)]
        async void load(OvkApiHub api)
        {
            var r = await api.GetDialogsList();
            foreach (var x in r)
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

        void Send(int peerId, string name)
        {
            dialogOverlay.Push(new SendDialog(name, () =>
            {
                api.SendWall(peerId, ownerId, postId);
            }));
        }
    }
}
