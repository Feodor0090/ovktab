using osu.Game.Overlays.Dialog;
using System;
using osu.Framework.Graphics.Sprites;


namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class SendDialog : PopupDialog
    {
        public SendDialog(string chat, Action callback)
        {
            Icon = FontAwesome.Solid.PaperPlane;
            HeaderText = "Send this post to the chat below?";
            BodyText = chat;
            Buttons = new PopupDialogButton[]
            {
                new PopupDialogOkButton
                {
                    Text = @"Send",
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
