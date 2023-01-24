using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Dialog;
using osu.Game.Rulesets.OvkTab.API;
using System;
using osu.Framework.Allocation;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public partial class RepostDialog : PopupDialog
    {
        private readonly int ownerId;
        private readonly int postId;
        private readonly LoadingLayer? newsLoading;
        private readonly Action<(int?, int?)?>? callback;

        [Resolved]
        private IOvkApiHub? api { get; set; }

        public RepostDialog(int ownerId, int postId, LoadingLayer? newsLoading, Action<(int?, int?)?>? callback = null)
        {
            this.ownerId = ownerId;
            this.postId = postId;
            this.newsLoading = newsLoading;
            this.callback = callback;
            Icon = FontAwesome.Solid.Bullhorn;
            HeaderText = "Confirm the repost";
            BodyText = "Everybody will see it. Do you want it?";

            Buttons = new PopupDialogButton[]
            {
                new PopupDialogOkButton
                {
                    Text = @"Yes. Totally. Repost it.",
                    Action = repost
                },
                new PopupDialogCancelButton
                {
                    Text = @"Shit, go back!",
                },
            };
        }

        private async void repost()
        {
            if (ownerId == 0 || postId == 0 || api == null)
                return;

            if (newsLoading != null) Schedule(newsLoading.Show);
            var r = await api.Repost(ownerId, postId);
            callback?.Invoke(r);
        }
    }
}
