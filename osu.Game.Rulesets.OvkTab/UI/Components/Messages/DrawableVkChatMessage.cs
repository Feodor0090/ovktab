using osu.Framework.Allocation;
using osu.Game.Rulesets.OvkTab.API;
using System;
using System.Collections.Generic;
using System.Linq;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Messages
{
    public class DrawableVkChatMessage : DrawableVkMessage
    {
        private readonly IEnumerable<SimpleVkUser> allUsers;
        private readonly Message msg;

        public DrawableVkChatMessage(SimpleVkUser user, Message msg, IEnumerable<SimpleVkUser> allUsers) : base(user)
        {
            this.msg = msg;
            this.allUsers = allUsers;
        }

        [BackgroundDependencyLoader(true)]
        void load()
        {
            AddContent(msg.Text, msg.Attachments, msg.Date ?? DateTime.Now);

            if (msg.ReplyMessage != null)
            {
                content.Add(new DrawableVkChatMessage(allUsers.Where(x => x.id == msg.ReplyMessage.FromId).First(), msg.ReplyMessage, allUsers));
            }
            if (msg.ForwardedMessages != null)
            {
                foreach (var m in msg.ForwardedMessages)
                {
                    content.Add(new DrawableVkChatMessage(allUsers.Where(x => x.id == m.FromId).First(), m, allUsers));
                }
            }
        }
    }
}
