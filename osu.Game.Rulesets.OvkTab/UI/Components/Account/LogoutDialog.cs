using osu.Framework.Graphics.Sprites;
using osu.Game.Overlays.Dialog;
using System;

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
