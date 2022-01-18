using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Cursor;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets.OvkTab.UI.Components;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics;
using VkNet.Model;
using static osu.Game.Rulesets.OvkTab.OvkApiHub;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class DrawableVkChatMessage : DrawableVkMessage
    {
        private IEnumerable<SimpleUser> allUsers;
        private Message msg;

        public DrawableVkChatMessage(SimpleUser user, Message msg, IEnumerable<SimpleUser> allUsers) : base(user)
        {
            this.msg = msg;
            this.allUsers = allUsers;
        }

        [BackgroundDependencyLoader(true)]
        void load(OsuGame game, LargeTextureStore lts)
        {
            AddContent(msg.Text, msg.Attachments, game, lts);

            if(msg.ReplyMessage != null)
            {
                content.Add(new DrawableVkChatMessage(allUsers.Where(x => x.id == msg.ReplyMessage.FromId).First(), msg.ReplyMessage, allUsers));
            }
            if(msg.ForwardedMessages!=null)
            {
                foreach(var m in msg.ForwardedMessages)
                {
                    content.Add(new DrawableVkChatMessage(allUsers.Where(x => x.id == m.FromId).First(), m, allUsers));
                }
            }
        }
    }
}
