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
using osu.Framework.Graphics.Cursor;

namespace osu.Game.Rulesets.OvkTab.UI
{
    [Cached]
    internal class OvkOverlay : FullscreenOverlay<OvkOverlayHeader>
    {
        [Cached]
        PopoverContainer pc;
            Container container;

        public LoadingLayer loginLoading;
        public LoadingLayer newsLoading;

        VkLoginBlock loginTab;
        OverlayScrollContainer newsTab;
        OverlayScrollContainer recommsTab;
        DialogsTab dialogsTab;
        OverlayScrollContainer friendsTab;
        OverlayScrollContainer groupsTab;

        Drawable[] tabs;

        private bool newsLoaded = false;
        private bool recommendedLoaded = false;
        [Cached]
        private readonly OvkApiHub apiHub;
        private readonly OvkTabRuleset ovkTabRuleset;
        readonly Bindable<bool> isLoggedIn;

        [Resolved]
        private NotificationOverlay nofs { get; set; }

        public OvkOverlay(OvkTabRuleset ovkTabRuleset) : base(OverlayColourScheme.Blue)
        {
            // API
            this.ovkTabRuleset = ovkTabRuleset;
            apiHub = new OvkApiHub(ovkTabRuleset);
            apiHub.OnLongPollFail += ex =>
              {
                  nofs.Post(new SimpleErrorNotification() { Text = "Longpoll failed: "+ex.Message });
              };
            isLoggedIn = apiHub.isLoggedIn.GetBoundCopy();
            isLoggedIn.BindValueChanged(e =>
            {
                if (e.NewValue == true)
                {
                    Schedule(() =>
                    {
                        loginTab.FadeOut(100);
                        dialogsTab.Start();
                        ChangeTab(Header.Current.Value);
                    });
                }
                else
                {
                    loginTab.ClearSession();
                    Schedule(() =>
                    {
                        newsTab.Child = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                        };
                        recommsTab.Child = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                        };
                        newsLoaded = false;
                        recommendedLoaded = false;
                        foreach (var d in tabs)
                        {
                            d.FadeOut(250, Easing.Out);
                        }
                        loginTab.Delay(500).FadeIn(500);
                        
                    });
                }
            });

            // Tabs
            loginTab = new VkLoginBlock(ovkTabRuleset);
            newsTab = new OverlayScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                }
            };
            newsTab.Hide();
            recommsTab = new OverlayScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                }
            };
            recommsTab.Hide();
            dialogsTab = new DialogsTab();
            dialogsTab.Hide();
            friendsTab = new OverlayScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
            };
            friendsTab.Hide();
            groupsTab = new OverlayScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new OsuButton
                    {
                        Width = 300,
                        Height = 100,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = "GROUPS"
                    }
                }
            };
            groupsTab.Hide();

            // Building layout
            tabs = new Drawable[] { newsTab, recommsTab, dialogsTab, friendsTab, groupsTab, loginTab };
            GridContainer grid;
            Add(pc = new PopoverContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = grid = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Content = new Drawable[][] {
                        new Drawable[] { Header },
                        new Drawable[] { container = new Container {
                            RelativeSizeAxes = Axes.Both,
                            Children = (IReadOnlyList<Drawable>)tabs.Clone(),

                        } }
                    },
                    RowDimensions = new[] { new Dimension(GridSizeMode.Absolute, 105), new Dimension(GridSizeMode.Distributed) }
                }
            });
            Header.Current.ValueChanged += e => Schedule(() => ChangeTab(e.NewValue));
            Add(loginLoading = new LoadingLayer(dimBackground: true));
            Add(newsLoading = new LoadingLayer(dimBackground: true));
        }
        private async void ChangeTab(OVKSections section)
        {
            if (!isLoggedIn.Value) return;
            container.ChangeChildDepth(tabs[(int)section], (float)-Clock.CurrentTime);
            foreach (var d in tabs)
            {
                d.FadeOut(250, Easing.Out);
            }
            tabs[(int)section].FadeIn(250, Easing.In);

            if (section == OVKSections.News && !newsLoaded)
            {
                newsLoaded = true;
                Schedule(() => newsLoading.FadeIn(200));
                var feed = await apiHub.LoadNews();
                var posts = feed.Select(x => new DrawableVkPost(x.Item1, x.Item2));
                Schedule(() => ((FillFlowContainer)newsTab.Child).AddRange(posts));
                Schedule(() => newsLoading.FadeOut(200));
            }
            if (section == OVKSections.Recommended && !recommendedLoaded)
            {
                recommendedLoaded = true;
                Schedule(() => newsLoading.FadeIn(200));
                var feed = await apiHub.LoadRecommended();
                var posts = feed.Select(x => new DrawableVkPost(x.Item1, x.Item2));
                Schedule(() => ((FillFlowContainer)recommsTab.Child).AddRange(posts));
                Schedule(() => newsLoading.FadeOut(200));
            }
            if (section == OVKSections.Friends)
            {
                var friends = await apiHub.GetFriendsList();
                var block = new FillFlowContainer()
                {
                    Padding = new MarginPadding() { Horizontal = 50 },
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new(10),
                    Children = friends.Select(x => new UserPanel(x)).ToArray()
                };
                Schedule(() => friendsTab.Child = block);
            }
            else { friendsTab.Clear(true); }
            if (section == OVKSections.Groups)
            {
                var groups = await apiHub.GetGroupsList();
                var block = new FillFlowContainer()
                {
                    Padding = new MarginPadding() { Horizontal = 50 },
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new(10),
                    Children = groups.Select(x => new UserPanel(x)).ToArray()
                };
                Schedule(() => groupsTab.Child = block);
            }
            else { groupsTab.Clear(true); }
        }

        protected override OvkOverlayHeader CreateHeader() => new OvkOverlayHeader();
    }
}
