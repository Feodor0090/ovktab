using osu.Game.Overlays.Dialog;
using System;
using osu.Framework.Allocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;

namespace osu.Game.Rulesets.OvkTab.UI
{
    public class LogoutDialog : PopupDialog
    {

        public LogoutDialog(Action callback)
        {
            Icon = FontAwesome.Solid.SignOutAlt;
            HeaderText = "Do you want to log out from this account?";
            BodyText = "Session data and token will be deleted.";
            Buttons = new PopupDialogButton[]
            {
                new PopupDialogOkButton
                {
                    Text = @"Log out",
                    Action = callback
                },
                new PopupDialogCancelButton
                {
                    Text = @"Cancel",
                },
            };
        }
    }
}
