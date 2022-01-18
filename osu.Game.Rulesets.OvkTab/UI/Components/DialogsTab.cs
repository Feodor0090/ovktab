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
using VkNet.Model;
using System.Threading.Tasks;
using static osu.Game.Rulesets.OvkTab.OvkApiHub;
using osu.Game.Graphics.Containers;
using osu.Framework.Utils;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    [Cached]
    internal class DialogsTab : Container
    {
        FillFlowContainer<DrawableDialog> dialogsList;
        Container dialogView;
        OsuTextBox messageInput;
        FillFlowContainer history;
        LoadingLayer listLoading;
        LoadingLayer historyLoading;
        HistoryScroll historyScroll;

        private Dictionary<int, SimpleUser> usersCache = new Dictionary<int, SimpleUser>();

        public Bindable<int> currentChat = new Bindable<int>(0);

        [Resolved]
        private OvkApiHub apiHub { get; set; }
        public DialogsTab()
        {
            RelativeSizeAxes = Axes.Both;
            listLoading = new LoadingLayer(dimBackground: true);
            historyLoading = new LoadingLayer(dimBackground: true);
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
                    messageInput,
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
        }

        [BackgroundDependencyLoader]
        void load()
        {
            apiHub.OnNewMessage += OnNewMessage;
            messageInput.OnCommit += MessageInput_OnCommit;
            apiHub.isLoggedIn.ValueChanged += x =>
            {
                if (x.NewValue == false) DeleteAll();
            };
        }

        private async void MessageInput_OnCommit(Framework.Graphics.UserInterface.TextBox sender, bool newText)
        {
            if (currentChat.Value != 0 && !string.IsNullOrWhiteSpace(sender.Text))
            {
                historyLoading.Show();
                bool ok = await apiHub.SendMessage(currentChat.Value, sender.Text);
                if (ok)
                {
                    sender.Text = String.Empty;
                }
                Schedule(() =>
                {
                    historyScroll.ScrollToEnd(true, true);
                    historyLoading.Hide();
                });
            }
        }

        private void OnNewMessage(OvkApiHub.LongpollMessage m)
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
                    id = apiHub.UserId;
                Message msg;
                // let's just load full object for now.
                if (m.extra.Count > 0)
                {
                    msg = apiHub.LoadMessage(m.messageId);
                    id = (int)msg.FromId;
                }
                else
                {
                    msg = new Message
                    {
                        Text = m.text
                    };
                }
                Schedule(() => history.Add(new DrawableVkChatMessage(usersCache[id], msg, usersCache.Values)));
            }
        }

        void DeleteAll()
        {
            dialogsList.Clear(true);
            history.Clear(true);
        }
        public async void Start()
        {
            Schedule(listLoading.Show);
            var list = await apiHub.GetDialogsList();
            var items = list.Select(x => new DrawableDialog(x));
            dialogsList.Clear(true);
            dialogsList.AddRange(items);
            foreach (var x in list)
            {
                if (x.Item1 != null) usersCache.TryAdd(x.Item1.id, x.Item1);
            }
            Schedule(listLoading.Hide);
        }

        public async void Open(int peerId)
        {
            currentChat.Value = 0;
            historyLoading.Show();
            history.Clear(true);
            var data = await apiHub.LoadHistory(peerId);
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
            });
            Schedule(historyLoading.Hide);
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
