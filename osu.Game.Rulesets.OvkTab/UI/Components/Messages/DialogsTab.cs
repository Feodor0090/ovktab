using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Messages
{
    [Cached]
    public class DialogsTab : Container
    {
        readonly FillFlowContainer<DrawableDialog> dialogsList;
        readonly Container dialogView;
        readonly OsuTextBox messageInput;
        readonly FillFlowContainer history;
        readonly LoadingLayer listLoading;
        readonly LoadingLayer historyLoading;
        readonly LoadingLayer longpollPending;
        readonly HistoryScroll historyScroll;

        private readonly Dictionary<int, SimpleVkUser> usersCache = new();

        public Bindable<int> currentChat = new(0);

        public Bindable<int> replyMessage = new(0);
        public Bindable<string> replyPreview = new();
        readonly AttachmentsPopoverContainer attsPopover;

        [Resolved]
        private IOvkApiHub ApiHub { get; set; }
        public DialogsTab()
        {
            RelativeSizeAxes = Axes.Both;
            listLoading = new LoadingLayer(dimBackground: true);
            historyLoading = new LoadingLayer(dimBackground: true);
            longpollPending = new LoadingLayer(dimBackground: true);
            dialogsList = new FillFlowContainer<DrawableDialog>
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new(5),
                Padding = new() { Vertical = 10, Right = 15 },
            };
            history = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new(10),
            };
            messageInput = new MessageInputBox();
            dialogView = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Width = 0.7f,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Children = new Drawable[]
                {
                    new Container
                    {
                        Padding = new MarginPadding{Bottom = 45 },
                        RelativeSizeAxes = Axes.Both,
                        Child = historyScroll = new HistoryScroll()
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = history,
                            ScrollbarVisible = true,
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 40,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Padding = new MarginPadding{ Left = 45 },
                        Child = messageInput,
                    },
                    attsPopover = new AttachmentsPopoverContainer(this)
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Size = new(40,40),
                    },
                    new IconTrianglesButton
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Size = new(40,40),
                        icon = FontAwesome.Solid.Bars,
                        iconSize = new(25),
                        Action = () => {
                            attsPopover.ShowPopover();
                        }

                    },
                }
            };


            Add(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Width = 0.3f,
                Children = new Drawable[]
                {
                    new OverlayScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = dialogsList,
                        ScrollbarVisible = true,
                        Padding = new() { Left = 5 }
                    },
                    listLoading
                }
            });
            Add(dialogView);
            Add(historyLoading);
            Add(longpollPending);
        }

        [BackgroundDependencyLoader]
        void load()
        {
            // longpoll
            ApiHub.OnNewMessage += OnNewMessage;
            ApiHub.OnMessageEdit += OnMessageEdit;
            messageInput.OnCommit += MessageInput_OnCommit;
            // handle logout
            ApiHub.LoggedUser.ValueChanged += x =>
            {
                if (x.NewValue == null) ClearContents();
            };
            // handle network failures
            ApiHub.IsLongpollFailing.BindValueChanged(e => Schedule(() =>
            {
                if (e.NewValue)
                    longpollPending.Show();
                else
                    longpollPending.Hide();
            }), true);
            ApiHub.IsLongpollFailing.BindValueChanged(e =>
            {
                if (!e.NewValue)
                {
                    if (currentChat.Value != 0)
                    {
                        Schedule(() => Open(currentChat.Value));
                        LoadDialogsList();
                    }
                }
            }, false);
        }

        private void OnMessageEdit(LongpollMessageEdit obj)
        {
        }

        public string TypedText
        {
            get => messageInput.Text;
            set => messageInput.Text = value;
        }

        private async void MessageInput_OnCommit(Framework.Graphics.UserInterface.TextBox sender, bool newText)
        {
            if (currentChat.Value != 0 && !string.IsNullOrWhiteSpace(sender.Text))
            {
                historyLoading.Show();
                bool ok = await ApiHub.SendMessage(currentChat.Value, sender.Text, replyMessage.Value);
                if (ok)
                {
                    sender.Text = string.Empty;
                    replyMessage.Value = 0;
                }
                Schedule(() =>
                {
                    historyScroll.ScrollToEnd(true, true);
                    historyLoading.Hide();
                });
            }
        }

        private void OnNewMessage(LongpollMessage m)
        {
            DrawableDialog dialog = dialogsList.Where(x => x.peerId == m.targetId).FirstOrDefault();
            if (dialog != null)
            {
                Schedule(() =>
                {
                    dialogsList.ChangeChildDepth(dialog, (float)Clock.CurrentTime);
                    dialogsList.SetLayoutPosition(dialog, (float)-Clock.CurrentTime);
                    dialog.Update(m.text, m.time);
                });
            }
            if (m.targetId == currentChat.Value)
            {
                int id;
                if (m.fromId != 0)
                    id = m.fromId;
                else
                    id = ApiHub.UserId;
                Message msg;
                // let's just load full object for now.
                if (m.extra.Count > 0)
                {
                    msg = ApiHub.LoadMessage(m.messageId);
                    id = (int)msg.FromId;
                }
                else
                {
                    msg = new Message
                    {
                        Text = m.text,
                        Id = m.messageId,
                    };
                }
                Schedule(() => history.Add(new DrawableVkChatMessage(usersCache[id], msg, usersCache.Values)));
            }
        }

        void ClearContents()
        {
            currentChat.Value = 0;
            dialogsList.Clear(true);
            history.Clear(true);
        }
        public async void LoadDialogsList()
        {
            Schedule(listLoading.Show);
            var list = await ApiHub.GetDialogsList();
            var items = list.Select(x => new DrawableDialog(x));
            Schedule(() =>
            {
                dialogsList.Clear(true);
                dialogsList.AddRange(items);
            });
            foreach (var x in list)
            {
                if (x.Item1 != null) usersCache.TryAdd(x.Item1.id, x.Item1);
            }
            Schedule(listLoading.Hide);
        }

        public async void Open(int peerId)
        {
            currentChat.Value = 0;
            replyMessage.Value = 0;
            historyLoading.Show();
            history.Clear(true);
            var data = await ApiHub.LoadHistory(peerId);
            var msgs = data.Item1;
            history.AddRange(msgs.Select(x => new DrawableVkChatMessage(x.Item1, x.Item2, data.Item2)).Reverse());
            foreach (var x in data.Item2)
            {
                usersCache.TryAdd(x.id, x);
            }
            await Task.Delay(250);
            Schedule(() =>
            {
                historyScroll.ScrollToEnd(true, true);
                historyLoading.Hide();
            });
            currentChat.Value = peerId;
        }

        private class MessageInputBox : OsuTextBox
        {
            public MessageInputBox()
            {
                Anchor = Anchor.BottomCentre;
                Origin = Anchor.BottomCentre;
                Position = new osuTK.Vector2(0);
                RelativeSizeAxes = Axes.X;
                PlaceholderText = "type your message";
                ReleaseFocusOnCommit = false;
            }

            public override bool RequestsFocus => true;
        }

        private class AttachmentsPopoverContainer : Drawable, IHasPopover
        {
            DialogsTab tab;
            public AttachmentsPopoverContainer(DialogsTab dialogsTab)
            {
                tab = dialogsTab;
            }
            public Popover GetPopover()
            {
                return new AttachmentsPopover(tab);
            }
        }

        private class HistoryScroll : UserTrackingScrollContainer
        {
            private const float auto_scroll_leniency = 100f;

            private float? lastExtent;

            protected override void OnUserScroll(float value, bool animated = true, double? distanceDecay = default)
            {
                base.OnUserScroll(value, animated, distanceDecay);
                lastExtent = null;
            }

            protected override void Update()
            {
                base.Update();

                if (UserScrolling && IsScrolledToEnd(auto_scroll_leniency))
                    CancelUserScroll();

                bool requiresScrollUpdate = !UserScrolling && (lastExtent == null || Precision.AlmostBigger(ScrollableExtent, lastExtent.Value));

                if (requiresScrollUpdate)
                {
                    Schedule(() =>
                    {
                        if (!UserScrolling)
                        {
                            if (Current < ScrollableExtent)
                                ScrollToEnd();
                            lastExtent = ScrollableExtent;
                        }
                    });
                }
            }
        }
    }
}
