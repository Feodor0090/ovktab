using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.OvkTab.API;
using System;
using System.Collections.Generic;
using System.Linq;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Messages
{
    public class DrawableVkChatMessage : DrawableVkMessage, IHasPopover
    {
        private readonly IEnumerable<SimpleVkUser> allUsers;
        private Message msg;
        private readonly bool canReplyOnThis;

        Bindable<int> replyId;
        Bindable<string> replyPreview;

        public int MessageId { get => (int)msg.Id; }

        [Resolved(canBeNull: true)]
        private DialogsTab tab { get; set; }

        public DrawableVkChatMessage(SimpleVkUser user, Message msg, IEnumerable<SimpleVkUser> allUsers, bool isRoot = true) : base(user)
        {
            this.msg = msg;
            this.allUsers = allUsers;
            canReplyOnThis = isRoot;
        }

        [BackgroundDependencyLoader(true)]
        void load()
        {
            AddContent(msg.Text, msg.Attachments, msg.Date ?? DateTime.Now, allUsers);

            if (msg.ReplyMessage != null)
            {
                content.Add(new DrawableVkChatMessage(allUsers.Where(x => x.id == msg.ReplyMessage.FromId).First(), msg.ReplyMessage, allUsers, false));
            }
            if (msg.ForwardedMessages != null)
            {
                foreach (var m in msg.ForwardedMessages)
                {
                    content.Add(new DrawableVkChatMessage(allUsers.Where(x => x.id == m.FromId).First(), m, allUsers, false));
                }
            }
            if (tab != null)
            {
                replyPreview = tab.replyPreview.GetBoundCopy();
                replyId = tab.replyMessage.GetBoundCopy();
                replyId.ValueChanged += e =>
                {
                    background.FadeColour(e.NewValue == msg.Id ? Colour4.DeepSkyBlue : Colour4.Black, 100);
                    triangles.FadeTo(e.NewValue == msg.Id ? 1 : 0, 100);
                };
            }
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (!canReplyOnThis || replyId == null) return false;

            if (e.Button == osuTK.Input.MouseButton.Right)
            {
                IOvkApiHub api = Dependencies.Get<IOvkApiHub>();
                if (user.id == api.UserId)
                {
                    this.ShowPopover();
                    return true;
                }
            }
            return false;
        }
        protected override bool OnClick(ClickEvent e)
        {

            if (!canReplyOnThis || replyId == null) return false;

            if (e.Button == osuTK.Input.MouseButton.Left)
            {
                replyId.Value = (int)msg.Id;
                replyPreview.Value = user.name + ": " + msg.Text;
            } else if(e.Button == osuTK.Input.MouseButton.Right)
            {
                IOvkApiHub api = Dependencies.Get<IOvkApiHub>();
                if(user.id == api.UserId)
                {
                    this.ShowPopover();
                }
            }

            return true;
        }


        internal void UpdateContent(Message message)
        {
            Schedule(() =>
            {
                content.Clear(true);
                header.Clear(true);
                msg = message;
                load();
                header.Add(new OsuSpriteText
                {
                    Font = OsuFont.GetFont(size: 18),
                    Text = $"/edited/",
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Colour = Colour4.Gray
                });
            });
        }

        public Popover GetPopover() => new MessageEditPopover(tab.currentChat.Value, MessageId, msg.ConversationMessageId ?? 0, msg.Text, () =>
        {
            this.Expire();
        });
    }
}
