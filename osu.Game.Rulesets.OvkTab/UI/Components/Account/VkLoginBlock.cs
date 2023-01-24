using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Chat;
using osu.Game.Rulesets.OvkTab.API;
using System;
using System.Threading.Tasks;
using osu.Game.Graphics.UserInterfaceV2;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Account
{
    internal partial class VkLoginBlock : Container
    {
        private OsuTextBox login;
        private OsuTextBox password;
        private OsuSpriteText errorText;

        [Resolved]
        private IOvkApiHub api { get; set; }

        [Resolved]
        private OvkOverlay ovk { get; set; }

        private OvkTabConfig? config;
        private readonly OvkTabRuleset? ruleset;

        private readonly Bindable<bool> keepSession = new();

        public VkLoginBlock(OvkTabRuleset? ruleset)
        {
            this.ruleset = ruleset;
        }

        [BackgroundDependencyLoader]
        void load(IRulesetConfigCache? c)
        {
            RelativeSizeAxes = Axes.Both;
            if (ruleset != null)
                config = c?.GetConfigFor(ruleset) as OvkTabConfig;
            string loginStr = config?.Get<string>(OvkTabRulesetSetting.Login) ?? string.Empty;
            Add(new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 300,
                AutoSizeAxes = Axes.Y,
                Spacing = new osuTK.Vector2(0, 5),
                Children = new Drawable[]
                {
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
                        Text = loginStr,
                        TabbableContentContainer = this,
                    },
                    password = new OsuPasswordTextBox
                    {
                        PlaceholderText = "password",
                        RelativeSizeAxes = Axes.X,
                        TabbableContentContainer = this,
                    },
                    new OsuCheckbox
                    {
                        LabelText = "Save session",
                        Current = keepSession
                    },
                    new RoundedButton
                    {
                        Text = "Log in",
                        RelativeSizeAxes = Axes.X,
                        Action = auth,
                        Height = 40,
                    },
                    errorText = new OsuSpriteText
                    {
                        Font = OsuFont.GetFont(weight: FontWeight.Regular, size: 16),
                        Colour = osuTK.Graphics.Color4.Red,
                        AllowMultiline = true,
                    }
                }
            });
            Add(new TextFlowContainer
            {
                Origin = Anchor.BottomCentre,
                Anchor = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Text = "This is an experimental project, that works via multiple hacks." +
                       " If you experience problems with the game, uninstall this extension before attempting to diagnose them. 2FA not supported, you will have to disable it. "
                       + "Your login, id and token will be stored in osu! database as plain text! No encryption yet, at all!",
                Position = new(0, -50),
                TextAnchor = Anchor.TopCentre,
                Padding = new() { Horizontal = 40 }
            });
            Add(new RoundedButton
            {
                Text = "GitHub page",
                Action = () => { Dependencies.Get<OsuGame>().HandleLink(new LinkDetails(LinkAction.External, "https://github.com/Feodor0090/ovktab")); },
                Height = 40,
                Width = 200,
                Position = new(0, -5),
                Origin = Anchor.BottomCentre,
                Anchor = Anchor.BottomCentre,
            });
            password.OnCommit += (_, _) => auth();
            errorText.Hide();
            if (config == null) return;

            // auto-login section
            if (config.Get<int>(OvkTabRulesetSetting.Id) != 0 && !string.IsNullOrEmpty(config.Get<string>(OvkTabRulesetSetting.Token)))
            {
                ovk.loginLoading.Show();
                Task.Run(() =>
                {
                    try
                    {
                        api.Auth(config.Get<int>(OvkTabRulesetSetting.Id), config.Get<string>(OvkTabRulesetSetting.Token));
                    }
                    catch
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
            if (config == null) return;
            config.SetValue(OvkTabRulesetSetting.Id, 0);
            config.SetValue(OvkTabRulesetSetting.Token, string.Empty);
        }

        private async void auth()
        {
            errorText.Hide();
            ovk.loginLoading.Show();
            await Task.Run(() =>
            {
                try
                {
                    api.Auth(login.Current.Value, password.Current.Value);

                    if (config != null)
                    {
                        config.SetValue(OvkTabRulesetSetting.Login, login.Current.Value);

                        if (keepSession.Value)
                        {
                            config.SetValue(OvkTabRulesetSetting.Id, api.UserId);
                            config.SetValue(OvkTabRulesetSetting.Token, api.Token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Schedule(() =>
                    {
                        errorText.Text = ex.Message;
                        errorText.Show();
                        password.FlashColour(Colour4.Red, 750);
                    });
                }
            });

            Schedule(ovk.loginLoading.Hide);
        }
    }
}
