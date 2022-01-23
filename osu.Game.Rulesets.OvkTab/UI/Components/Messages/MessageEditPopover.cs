﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.Misc;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Messages
{
    public class MessageEditPopover : OsuPopover
    {
        [Resolved]
        IOvkApiHub api { get; set; }

        OsuTextBox textBox;

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
            });
            Add(new DangerousTriangleButton
            {
                Position = new(125, 45),
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
