using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.API;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Account
{
    public partial class HeaderProfileBadge : Container
    {
        public HeaderProfileBadge()
        {
            cont = new FillFlowContainer()
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Direction = FillDirection.Horizontal,
                Spacing = new(10, 0)
            };
            RelativeSizeAxes = Axes.Y;
            AutoSizeAxes = Axes.X;
            Add(cont);
            cont.Hide();
            Alpha = 1;
        }

        private readonly FillFlowContainer cont;

        [Resolved(canBeNull: true)]
        private IDialogOverlay dialogOverlay { get; set; }

        IOvkApiHub api;

        [BackgroundDependencyLoader]
        void load(IOvkApiHub ovk)
        {
            api = ovk;
            ovk.LoggedUser.BindValueChanged(e =>
            {
                if (e.NewValue != null)
                    Schedule(() => OnLogIn(e.NewValue));
                else
                    cont.FadeOut(250);
            }, true);
            ovk.IsLongpollFailing.ValueChanged += e =>
            {
                if (!e.NewValue && ovk.LoggedUser.Value != null)
                {
                    Schedule(() => OnLogIn(ovk.LoggedUser.Value));
                }
                else
                {
                    Schedule(OnConnectionFail);
                }
            };
        }

        private void OnConnectionFail()
        {
            LoadingSpinner spinner;
            cont.Children = new Drawable[]
            {
                spinner = new LoadingSpinner(true)
                {
                    Size = new(40),
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                },
                new OsuSpriteText()
                {
                    Text = "Connection is failing...",
                    Position = new(65, 0),
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Font = OsuFont.GetFont(size: 18),
                    Colour = Colour4.LightPink,
                },
                new DangerousTriangleButton()
                {
                    Size = new(100, 40),
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Text = "Log out",
                    Action = () =>
                    {
                        if (dialogOverlay == null)
                            api.Logout();
                        else
                            dialogOverlay.Push(new LogoutDialog(api.Logout));
                    }
                }
            };
            cont.FadeIn(1000);
            spinner.Show();
        }

        public void OnLogIn(SimpleVkUser user)
        {
            Container avCont;
            DangerousTriangleButton button;
            cont.Children = new Drawable[]
            {
                avCont = new()
                {
                    Size = new(50),
                    Scale = new(0.8f),
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                },
                new OsuSpriteText()
                {
                    Text = user.name,
                    Position = new(65, 0),
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Font = OsuFont.GetFont(size: 18),
                },
                button = new()
                {
                    Size = new(40, 40),
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Action = () =>
                    {
                        if (dialogOverlay == null)
                            api.Logout();
                        else
                            dialogOverlay.Push(new LogoutDialog(api.Logout));
                    }
                }
            };
            button.Add(new SpriteIcon
            {
                Icon = FontAwesome.Solid.SignOutAlt,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new(18),
            });
            cont.FadeIn(1000);
            LoadComponentAsync(new DrawableVkAvatar(user) { Position = new(25) }, x => avCont.Add(x));
        }
    }
}
