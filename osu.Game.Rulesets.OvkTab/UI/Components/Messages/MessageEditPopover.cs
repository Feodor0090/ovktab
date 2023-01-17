using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.OvkTab.API;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Messages
{
    public partial class MessageEditPopover : OsuPopover
    {
        [Resolved]
        private IOvkApiHub api { get; set; }

        readonly OsuTextBox textBox;

        public MessageEditPopover(int peerId, int messageId, long convMessageId, string oldText, Action onDelete)
        {
            Add(textBox = new OsuTextBox
            {
                Width = 700,
                Position = new(0, 0),
                Text = oldText,
            });
            Add(new TriangleButton
            {
                Position = new(0, 45),
                Size = new(120, 40),
                Text = "Apply changes",
                Action = async () =>
                {
                    if (await api.EditMessage(peerId, convMessageId, messageId, textBox.Text, true, true))
                        this.HidePopover();
                    else
                        textBox.FlashColour(Colour4.Red, 500);
                }
            });
            Add(new DangerousTriangleButton
            {
                Position = new(125, 45),
                Size = new(140, 40),
                Text = "Apply & drop reply",
                Action = async () =>
                {
                    if (await api.EditMessage(peerId, convMessageId, messageId, textBox.Text, false, true))
                        this.HidePopover();
                    else
                        textBox.FlashColour(Colour4.Red, 500);
                }
            });
            Add(new DangerousTriangleButton
            {
                Position = new(540, 45),
                Size = new(160, 40),
                Text = "Delete this message",
                Action = async () =>
                {
                    if (await api.DeleteMessage(peerId, (ulong)convMessageId, (ulong)messageId))
                        onDelete();
                }
            });
        }
    }
}
