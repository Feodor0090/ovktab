using osu.Framework.IO.Network;
using osu.Game.Overlays.Notifications;
using System;
using System.IO;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.OvkTab.API
{
    /// <summary>
    /// VK document, that can be downloaded and imported into the game.
    /// </summary>
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

        /// <summary>
        /// Begin downloading.
        /// </summary>
        public void Download()
        {
            string file = Path.GetTempFileName();

            File.Move(file, filename = Path.ChangeExtension(file, ext));

            var request = new FileWebRequest(filename, docUrl);

            DocDownloadNotification nof = new(docName);

            nof.CancelRequested += () =>
            {
                request.Abort();
                OnFail();
                return true;
            };

            request.DownloadProgress += (a, b) => { nof.Progress = (float)a / b; };
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

        private class DocDownloadNotification : ProgressNotification
        {
            public override bool IsImportant => false;

            public DocDownloadNotification(string docName)
            {
                Text = $"Downloading {docName}";
                CompletionText = $"{docName} downloaded!";
                State = ProgressNotificationState.Active;
                CompletionClickAction = () => true;
            }

            protected override Notification CreateCompletionNotification() => new ProgressCompletionNotification
            {
                Activated = CompletionClickAction,
                Text = CompletionText
            };
        }
    }
}
