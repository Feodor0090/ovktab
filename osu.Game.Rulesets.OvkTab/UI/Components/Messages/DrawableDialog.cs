using osu.Framework.Graphics.Containers;
using System;
using VkNet.Model;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.Containers;
using osu.Game.Beatmaps.ControlPoints;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Input.Events;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    internal class DrawableDialog : BeatSyncedContainer
    {
        private readonly SimpleVkUser user;
        private readonly ConversationAndLastMessage msg;
        private DrawableVkAvatar avatar;
        private OsuSpriteText userName;
        private Drawable unreadMark;
        private Drawable selectionBox;
        private Drawable hoverBox;

        public int peerId;
        private Bindable<int> activeChat;
        private OsuSpriteText messageText;
        private OsuSpriteText time;

        public DrawableDialog((SimpleVkUser, ConversationAndLastMessage) x)
        {
            user = x.Item1;
            msg = x.Item2;
            peerId = (int)x.Item2.Conversation.Peer.Id;
            Height = 60;
            RelativeSizeAxes = Axes.X;
            Masking = true;
            CornerRadius = 15;
        }

        [BackgroundDependencyLoader]
        void load(OsuColour colour, DialogsTab dialogs)
        {
            var font = OsuFont.GetFont(size: 20);
            Children = new Drawable[]
            {
                hoverBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colour.BlueDarker,
                },
                selectionBox = new Container {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[] {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colour.BlueDark,
                        },
                        new KiaiTriangles(colour.Blue3, colour.BlueDarker, 1f),
                    }
                },
                new DialogPreview(user?.name ?? msg.Conversation.ChatSettings?.Title,msg.LastMessage.Date??DateTime.Now, peerId, msg.LastMessage.Text, dialogs, hoverBox, colour.Blue, 
                out userName, out time, out unreadMark, out messageText),
            };
            if (!msg.Conversation.UnreadCount.HasValue || msg.Conversation.UnreadCount == 0)
            {
                unreadMark.Hide();
            }
            selectionBox.Hide();
            hoverBox.Hide();
            activeChat = dialogs.currentChat.GetBoundCopy();
            activeChat.ValueChanged += ActiveChat_ValueChanged;

            avatar = user == null ?
                new DrawableVkAvatar(msg.Conversation.ChatSettings?.Title, msg.Conversation.ChatSettings?.Photo?.Photo100.AbsoluteUri)
                {
                    Position = new(30),
                }
                : new DrawableVkAvatar(user)
                {
                    Position = new(30),
                };
            LoadComponentAsync(avatar, x => { Add(x); avatar = x; });
        }

        private void ActiveChat_ValueChanged(ValueChangedEvent<int> e)
        {
            if (e.NewValue == peerId)
            {
                selectionBox.FadeIn(500);
                unreadMark.ScaleTo(0, 500, Easing.InBounce);
            }
            else
                selectionBox.FadeOut(500);
        }

        public void Update(string newText, DateTime newTime)
        {
            messageText.Text = newText;
            time.Text = $"{newTime:d MMMM HH:mm}";
            if (peerId != activeChat.Value)
            {
                unreadMark.Show();
                unreadMark.ScaleTo(0).ScaleTo(1, 500, Easing.InBounce);
            }
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            if (effectPoint.KiaiMode)
            {
                double b4 = timingPoint.BeatLength / 4d;
                avatar?.ScaleTo(1.02f, b4 * 3, Easing.Out).Then().ScaleTo(1f, b4, Easing.OutQuint);
                userName?.FlashColour(Colour4.FromHex("BBB"), timingPoint.BeatLength);
            }
        }

        private sealed class DialogPreview : OsuClickableContainer
        {
            private readonly Drawable hover;
            public DialogPreview(string title, DateTime time, int id, string text, DialogsTab dialogs, Drawable hoverBox, Colour4 unreadMarkColor, 
                out OsuSpriteText userNameText, out OsuSpriteText timeText, out Drawable unreadCircle, out OsuSpriteText textText)
            {
                hover = hoverBox;
                var font = OsuFont.GetFont(size: 20);
                Padding = new()
                {
                    Left = 65,
                    Right = 10
                };
                RelativeSizeAxes = Axes.Both;
                Children = new Drawable[] {
                    userNameText = new OsuSpriteText()
                    {
                        Text = title,
                        Position = new(0, 10),
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.X,
                        Font = font,
                        Truncate = true
                    },
                    timeText = new OsuSpriteText()
                    {
                        Text = $"{time:d MMMM HH:mm} ",
                        Position = new(0, 30),
                        Origin = Anchor.CentreLeft,
                        Font = font,
                    },
                    unreadCircle = new Circle()
                    {
                        Position = new(-5, 0),
                        Origin = Anchor.CentreRight,
                        Anchor = Anchor.CentreRight,
                        Colour = unreadMarkColor,
                        Size = new(10),
                    },
                    textText = new OsuSpriteText()
                    {
                        Text = text,
                        Position = new(0, 50),
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.X,
                        Font = font,
                        Truncate = true
                    }
                };
                Action = () => dialogs.Open(id);
            }

            protected override bool OnHover(HoverEvent e)
            {
                hover.FadeIn(200);
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                hover.FadeOut(500);
                base.OnHoverLost(e);
            }
        }
    }
}
