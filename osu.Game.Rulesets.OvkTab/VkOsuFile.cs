using osu.Framework.IO.Network;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Overlays.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.OvkTab
{
    public class VkOsuFile
    {
        public string docUrl;
        public string ext;
        public string docName;
        private string filename;
        public Action<Notification> PostNotification;
        public Action OnFail;
        public Action OnOk;
        private readonly OsuGame game;

        public VkOsuFile(OsuGame osuGame)
        {
            game = osuGame;
        }

        public void Download()
        {
            string file = Path.GetTempFileName();

            File.Move(file, filename = Path.ChangeExtension(file, ext));

            var request = new FileWebRequest(filename, docUrl);

            DownloadNotification nof = new()
            {
                Text = $"Downloading {docName}",
            };

            request.DownloadProgress += (a, b) => { nof.Progress = (float)a / b; };
            nof.CancelRequested += () =>
            {
                request.Abort();
                OnFail();
                return true;
            };
            nof.State = ProgressNotificationState.Active;
            nof.CompletionClickAction = () => true;
            nof.CompletionText = "Document downloaded!";
            request.Failed += e =>
            {
                nof.State = ProgressNotificationState.Cancelled;
                OnFail();
            };
            request.Finished += () =>
            {
                Task.Factory.StartNew(async () =>
                {
                    nof.State = ProgressNotificationState.Completed;
                    await game.Import(filename);
                    OnOk();
                }, TaskCreationOptions.LongRunning);
            };

            PostNotification?.Invoke(nof);
            request.PerformAsync();
        }



        private class DownloadNotification : ProgressNotification
        {
            public override bool IsImportant => false;

            protected override Notification CreateCompletionNotification() => new ProgressCompletionNotification
            {
                Activated = CompletionClickAction,
                Text = CompletionText
            };
        }
    }
}
