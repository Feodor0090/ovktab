using System;
using System.Linq;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Game.Rulesets.OvkTab.UI.Components;
using osu.Framework.Graphics.Cursor;
using osu.Game.Rulesets.OvkTab.UI.Components.Account;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.Messages;
using osu.Game.Rulesets.OvkTab.UI.Components.Posts;
using osu.Game.Rulesets.OvkTab.UI.Components.Misc;

namespace osu.Game.Rulesets.OvkTab.UI
{
    [Cached]
    public class OvkOverlay : FullscreenOverlay<OvkOverlayHeader>
    {
        [Cached]
        readonly PopoverContainer pc;
        readonly Container container;

        public LoadingLayer loginLoading;
        public LoadingLayer newsLoading;

        readonly VkLoginBlock loginTab;
        readonly OverlayScrollContainer newsTab;
        readonly OverlayScrollContainer recommsTab;
        readonly DialogsTab dialogsTab;
        readonly OverlayScrollContainer friendsTab;
        readonly OverlayScrollContainer groupsTab;
        readonly Drawable[] tabs;

        private bool newsLoaded = false;
        private bool recommendedLoaded = false;

        [Cached(typeof(IOvkApiHub))]
        private readonly IOvkApiHub apiHub;
        readonly Bindable<SimpleVkUser> logged;

        [Resolved]
        private NotificationOverlay nofs { get; set; }

        public OvkOverlay(OvkTabRuleset ovkTabRuleset) : base(OverlayColourScheme.Blue)
        {
            // API
            apiHub = new OvkApiHub();
            apiHub.IsLongpollFailing.ValueChanged += e =>
              {
                  if(e.NewValue == true) nofs.Post(new SimpleErrorNotification() { Text = "Longpoll is failing. Check your connection." });
              };
            logged = apiHub.LoggedUser.GetBoundCopy();
            logged.BindValueChanged(e =>
            {
                if (e.NewValue != null)
                {
                    Schedule(() =>
                    {
                        loginTab.FadeOut(100);
                        dialogsTab.LoadDialogsList();
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
            newsTab = new FullsizeScroll();
            newsTab.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
            });
            recommsTab = new FullsizeScroll();
            recommsTab.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
            });
            dialogsTab = new DialogsTab();
            friendsTab = new FullsizeScroll();
            groupsTab = new FullsizeScroll();

            // Building layout
            tabs = new Drawable[] { newsTab, recommsTab, dialogsTab, friendsTab, groupsTab, loginTab };
            foreach(var d in tabs) 
                d.Hide();
            loginTab.Show();

            Add(pc = new PopoverContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    container = new Container {
                        Padding = new MarginPadding { Top = 102 },
                        RelativeSizeAxes = Axes.Both,
                        Children = tabs
                    },
                    Header
                }
            });
            
            Header.Current.ValueChanged += e => Schedule(() => ChangeTab(e.NewValue));
            Add(loginLoading = new LoadingLayer(dimBackground: true));
            Add(newsLoading = new LoadingLayer(dimBackground: true));
        }
        private async void ChangeTab(OVKSections section)
        {
            if (logged.Value == null) return;
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

        protected override OvkOverlayHeader CreateHeader() => new();
    }
}
