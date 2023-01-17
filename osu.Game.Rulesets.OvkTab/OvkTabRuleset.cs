// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.OvkTab.Beatmaps;
using osu.Game.Rulesets.OvkTab.UI;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.UI;
using osu.Game.Overlays.Notifications;
using osu.Framework.Allocation;
using osu.Framework.Platform;
using System.Reflection;
using osu.Game.Overlays;
using System;
using osu.Game.Overlays.Toolbar;
using System.Linq;
using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;
using osu.Framework.Logging;

namespace osu.Game.Rulesets.OvkTab
{
    public partial class OvkTabRuleset : Ruleset
    {
        public override string Description => "OVK";

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) => new OvkTabConfig(settings, RulesetInfo);

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) =>
            new DrawableOvkTabRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) =>
            new OvkTabBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(IWorkingBeatmap beatmap) =>
            new OvkTabDifficultyCalculator(RulesetInfo, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            return Array.Empty<Mod>();
        }

        public override string ShortName => "OVK";

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.Z, OvkTabAction.Button1),
            new KeyBinding(InputKey.X, OvkTabAction.Button2),
        };

        public override Drawable CreateIcon() => new Icon(this);

        public partial class Icon : CompositeDrawable
        {
            private readonly OvkTabRuleset ruleset;

            public Icon(OvkTabRuleset ovkTab)
            {
                ruleset = ovkTab;
                RelativeSizeAxes = Axes.Both;

                InternalChildren = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Icon = FontAwesome.Regular.Circle,
                        RelativeSizeAxes = Axes.Both,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre
                    },
                    new SpriteIcon
                    {
                        Icon = FontAwesome.Brands.Vk,
                        RelativeSizeAxes = Axes.Both,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Size = new(0.5f),
                    },
                };
            }

            public static string ErrorMessage(string code)
                => $"Could not load OVK: Please report this to the OVK repository NOT the osu!lazer repository: Code {code}";

            [BackgroundDependencyLoader(permitNulls: true)]
            private void load(OsuGame game, GameHost host)
            {
                if (game is null) return;
                if (game.Dependencies.Get<OvkOverlay>() != null) return;

                var notifications = typeof(OsuGame).GetField("Notifications", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(game) as NotificationOverlay;

                if (notifications is null)
                {
                    return;
                }

                // https://github.com/ppy/osu/blob/edf5e558aca6cd75e70b510a5f0dd233d6cfcb90/osu.Game/OsuGame.cs#L790
                // contains overlays
                var overlayContent = typeof(OsuGame).GetField("overlayContent", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(game) as Container;

                if (overlayContent is null)
                {
                    Schedule(() => notifications.Post(new SimpleErrorNotification { Text = ErrorMessage("#OCNRE") }));
                    return;
                }

                // https://github.com/ppy/osu/blob/edf5e558aca6cd75e70b510a5f0dd233d6cfcb90/osu.Game/OsuGame.cs#L953
                // caches the overlay globally and allows us to run code when it is loaded
                var loadComponent = typeof(OsuGame).GetMethod("loadComponentSingleFile", BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(typeof(OvkOverlay));

                if (loadComponent is null)
                {
                    Schedule(() => notifications.Post(new SimpleErrorNotification { Text = ErrorMessage("#LCNRE") }));
                    return;
                }

                try
                {
                    loadComponent.Invoke(game,
                        new object[] { new OvkOverlay(ruleset), (Action<Drawable>)addOverlay, true }
                    );
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "LCIException from OVK");
                    Schedule(() => notifications.Post(new SimpleErrorNotification { Text = ErrorMessage("#LCIE") }));
                    return;
                }

                void addOverlay(Drawable overlay)
                {
                    overlayContent.Add(overlay);

                    // https://github.com/ppy/osu/blob/edf5e558aca6cd75e70b510a5f0dd233d6cfcb90/osu.Game/Overlays/Toolbar/Toolbar.cs#L89
                    // leveraging an "easy" hack to get the container with toolbar buttons
                    var userButton = typeof(Toolbar).GetField("userButton", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(game.Toolbar) as Drawable;

                    if (userButton is null || userButton.Parent is not FillFlowContainer buttonsContainer)
                    {
                        Schedule(() => notifications.Post(new SimpleErrorNotification { Text = ErrorMessage("#UBNRE") }));
                        overlayContent.Remove(overlay, true);
                        return;
                    }

                    var button = new OvkToolbarButton();
                    buttonsContainer.Insert(-1, button);

                    // https://github.com/ppy/osu/blob/edf5e558aca6cd75e70b510a5f0dd233d6cfcb90/osu.Game/OsuGame.cs#L855
                    // add overlay hiding, since osu does it manually
                    var singleDisplayOverlays = new string[] { "chatOverlay", "news", "dashboard", "beatmapListing", "changelogOverlay", "wikiOverlay" };
                    var overlays = singleDisplayOverlays.Select(name =>
                        typeof(OsuGame).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(game) as OverlayContainer
                    ).ToList();

                    if (game.Dependencies.TryGet<RankingsOverlay>(out var rov))
                    {
                        overlays.Add(rov);
                    }
                    else
                    {
                        Schedule(() => notifications.Post(new SimpleErrorNotification { Text = ErrorMessage("#ROVNRE") }));
                        overlayContent.Remove(overlay, true);
                        buttonsContainer.Remove(button, true);
                        return;
                    }

                    if (overlays.Any(x => x is null))
                    {
                        Schedule(() => notifications.Post(new SimpleErrorNotification { Text = ErrorMessage("#OVNRE") }));
                        overlayContent.Remove(overlay, true);
                        buttonsContainer.Remove(button, true);
                        return;
                    }

                    foreach (var i in overlays)
                    {
                        i.State.ValueChanged += v =>
                        {
                            if (v.NewValue != Visibility.Visible) return;

                            overlay.Hide();
                        };
                    }

                    ((OvkOverlay)overlay).State.ValueChanged += v =>
                    {
                        if (v.NewValue != Visibility.Visible) return;

                        foreach (var i in overlays)
                        {
                            i.Hide();
                        }

                        // https://github.com/ppy/osu/blob/edf5e558aca6cd75e70b510a5f0dd233d6cfcb90/osu.Game/OsuGame.cs#L896
                        // show above other overlays
                        if (overlay.IsLoaded)
                            overlayContent.ChangeChildDepth(overlay, (float)-Clock.CurrentTime);
                        else
                            overlay.Depth = (float)-Clock.CurrentTime;
                    };
                }
            }
        }
    }
}
