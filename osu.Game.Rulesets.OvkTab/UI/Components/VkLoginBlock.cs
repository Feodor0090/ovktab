using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Cursor;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    internal class VkLoginBlock : Container
    {
        private OsuTextBox login;
        private OsuTextBox password;
        private OsuSpriteText errorText;
        private ShakeContainer shake;

        [Resolved]
        private OvkApiHub api { get; set; }
        [Resolved]
        private OvkOverlay ovk { get; set; }
        private OvkTabConfig config;
        private OvkTabRuleset ruleset;

        private Bindable<bool> keepSession = new Bindable<bool>();
        public VkLoginBlock(OvkTabRuleset ruleset)
        {
            this.ruleset = ruleset;
        }

        [BackgroundDependencyLoader]
        void load(IRulesetConfigCache c)
        {
            
            RelativeSizeAxes = Axes.Both;
            config = c?.GetConfigFor(ruleset) as OvkTabConfig;
            Add(new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 300,
                AutoSizeAxes = Axes.Y,
                Spacing = new osuTK.Vector2(0, 5),
                Children = new Drawable[] {
                    new OsuSpriteText
                    {
                        Text = "Log into your VK account",
                        Margin = new MarginPadding { Bottom = 5 },
                        Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 18),
                    },
                    login = new OsuTextBox
                    {
                        PlaceholderText = "login",
                        RelativeSizeAxes = Axes.X,
                        Text = config.Get<string>(OvkTabRulesetSetting.Login),
                        TabbableContentContainer = this,
                    },
                    password = new OsuPasswordTextBox
                    {
                        PlaceholderText = "password",
                        RelativeSizeAxes = Axes.X,
                        TabbableContentContainer = this,
                    },
                    shake = new ShakeContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Child = new OsuButton
                        {
                            Text = "Log in",
                            RelativeSizeAxes = Axes.X,
                            Action = Auth
                        },
                    },
                    new OsuCheckbox
                    {
                        LabelText = "Save session",
                        Current = keepSession
                    },
                    errorText = new OsuSpriteText
                    {
                        Font = OsuFont.GetFont(weight: FontWeight.Regular, size: 16),
                        Colour = osuTK.Graphics.Color4.Red,
                        AllowMultiline = true,
                    }
                }
            });
            Add(new TextFlowContainer()
            {
                Origin = Anchor.BottomCentre,
                Anchor = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Text = "This is an experimental project, that works via multiple hacks." +
                " If you experience problems with the game, uninstall this extension before attempting to diagnose them.",
                Position = new(0, -10),
                TextAnchor = Anchor.TopCentre,
                Padding = new() { Horizontal = 40 }
            });
            password.OnCommit += (_, _) => Auth();
            errorText.Hide();
            if (config.Get<int>(OvkTabRulesetSetting.Id) != 0 && !string.IsNullOrEmpty(config.Get<string>(OvkTabRulesetSetting.Token)))
            {
                ovk.loginLoading.Show();
                Task.Run(() =>
                {
                    try
                    {
                        api.Auth(config.Get<int>(OvkTabRulesetSetting.Id), config.Get<string>(OvkTabRulesetSetting.Token));
                        Schedule(() => api.badge.OnLogIn(api.Current));
                    }
                    catch (Exception ex)
                    {
                        Schedule(() =>
                        {
                            errorText.Text = "Failed to restore session.";
                            errorText.Show();
                        });
                    }
                    Schedule(ovk.loginLoading.Hide);
                });
            }
        }

        public void ClearSession()
        {
            config.SetValue(OvkTabRulesetSetting.Id, 0);
            config.SetValue(OvkTabRulesetSetting.Token, string.Empty);
        }

        async void Auth()
        {
            errorText.Hide();
            ovk.loginLoading.Show();
            await Task.Run(() =>
            {
                try
                {
                    api.Auth(login.Current.Value, password.Current.Value);
                    config.SetValue(OvkTabRulesetSetting.Login, login.Current.Value);
                    if(keepSession.Value)
                    {
                        config.SetValue(OvkTabRulesetSetting.Id, api.UserId);
                        config.SetValue(OvkTabRulesetSetting.Token, api.Token);
                    }
                    Schedule(() => api.badge.OnLogIn(api.Current));
                }
                catch (Exception ex)
                {
                    Schedule(() =>
                    {
                        errorText.Text = ex.Message;
                        errorText.Show();
                        shake.Shake();
                    });
                }
            });

            Schedule(ovk.loginLoading.Hide);
        }
    }
}
