using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class HeaderProfileBadge : FillFlowContainer
    {
        public HeaderProfileBadge()
        {
            RelativeSizeAxes = Axes.Y;
            AutoSizeAxes = Axes.X;
            Direction = FillDirection.Horizontal;
            Spacing = new(10, 0);
        }
        [Resolved(canBeNull: true)] private DialogOverlay DialogOverlay { get; set; }
        OvkApiHub api;

        [BackgroundDependencyLoader]
        void load(OvkApiHub ovk)
        {
            api = ovk;
            ovk.loggedUser.ValueChanged += e =>
             {
                 if (e.NewValue != null)
                     OnLogIn(e.NewValue);
                 else
                     this.FadeOut(250);
             };
            Hide();
        }

        public void OnLogIn(SimpleVkUser user)
        {
            Container cont;
            DangerousTriangleButton button;
            Children = new Drawable[] {
                cont = new()
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
                        if (DialogOverlay == null)
                            api.Logout();
                        else
                            DialogOverlay.Push(new LogoutDialog(api.Logout));
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
            this.FadeIn(1000);
            LoadComponentAsync(new DrawableVkAvatar(user) { Position = new(25) }, x => cont.Add(x));
        }
    }
}
