using osu.Game.Overlays.Dialog;
using System;
using osu.Framework.Allocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.API;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public class RepostDialog : PopupDialog
    {

        public RepostDialog(int ownerId, int postId, OvkApiHub api, LoadingLayer newsLoading, Action<(int?, int?)?> callback)
        {
            Icon = FontAwesome.Solid.Bullhorn;
            HeaderText = "Confirm the repost";
            BodyText = "Everybody will see it. Do you want it?";
            Buttons = new PopupDialogButton[]
            {
                new PopupDialogOkButton
                {
                    Text = @"Yes. Totally. Repost it.",
                    Action = async () =>
                    {
                        if(ownerId==0 || postId==0) return;
                        if(api == null) return;
                        if(newsLoading!=null) Schedule(newsLoading.Show);
                        var r = await api.Repost(ownerId, postId);
                        callback(r);
                    }
                },
                new PopupDialogCancelButton
                {
                    Text = @"Shit, go back!",
                },
            };
        }
    }
}
