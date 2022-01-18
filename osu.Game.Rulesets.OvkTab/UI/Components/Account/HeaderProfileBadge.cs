using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using VkNet.Model;
using osu.Game.Online.Chat;
using osu.Game.Graphics.UserInterfaceV2;
using System.Threading.Tasks;
using osu.Game.Graphics.Sprites;

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
        [Resolved(canBeNull: true)] private DialogOverlay dialogOverlay { get; set; }
        OvkApiHub api;

        [BackgroundDependencyLoader]
        void load(OvkApiHub ovk)
        {
            api = ovk;
            ovk.badge = this;
            Hide();
        }

        public void OnLogOut()
        {
            this.FadeOut(250);
        }

        public void OnLogIn(SimpleVkUser user)
        {
            Clear(true);
            Container cont = new Container
            {
                Size = new(50),
                Scale = new(0.8f),
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
            };
            Add(cont);
            Add(new OsuSpriteText()
            {
                Text = user.name,
                Position = new(65, 0),
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Font = OsuFont.GetFont(size: 18),
            });
            var button = new DangerousTriangleButton()
            {
                Size = new(40, 40),
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Action = () =>
                {
                    if (dialogOverlay == null)
                    {
                        api.Logout();
                    }
                    else
                    {
                        dialogOverlay.Push(new LogoutDialog(api.Logout));
                    }
                }
            };
            Add(button);
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
