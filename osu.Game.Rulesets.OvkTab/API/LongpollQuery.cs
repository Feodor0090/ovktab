using Newtonsoft.Json;
using osu.Framework.IO.Network;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.OvkTab.API
{
    public sealed class LongpollQuery : IDisposable
    {
        private readonly OvkApiHub api;

        public LongpollQuery(OvkApiHub api)
        {
            this.api = api;
        }

        private bool stop = false;
        private WebRequest request = null;

        public async void Run()
        {
            // loop for reconnection to longpoll server
            while (!stop)
            {
                try
                {
                    var lpp = await api.ConnectToLongPoll();
                    api.IsLongpollFailing.Value = false;
                    string activeTs = lpp.Ts;
                    // loop for handling requests
                    while (!stop)
                    {
                        LongpollData ld;
                        using (request = new()
                        {
                            Method = HttpMethod.Get,
                            Url = $"https://{lpp.Server}?act=a_check&key={lpp.Key}&ts={activeTs}&wait=25&mode=2&version=3",
                            Timeout = 60000,
                            AllowRetryOnTimeout = true
                        })
                        {

                            request.Perform();
                            ld = JsonConvert.DeserializeObject<LongpollData>(request.GetResponseString());
                            request = null;
                        }
                        activeTs = ld.Ts;
                        if (!ld.HasUpdates)
                        {
                            continue;
                        }

                        var updates = ld.GetUpdates();

                        if (stop) return;
                        foreach (LongpollUpdate update in updates)
                        {
                            if (update.type == 4)
                            {
                                api.ReceiveNewMessage(new LongpollMessage(update));
                            }
                            else if (update.type == 5)
                            {
                                api.EditMessage(new LongpollMessageEdit(update));
                            }
                        }
                    }
                }
                catch
                {
                    if (stop) return;
                    api.IsLongpollFailing.Value = true;
                    await Task.Delay(10000);
                }
            }
        }

        public void Dispose()
        {
            api.IsLongpollFailing.Value = false;
            stop = true;
            request?.Abort();
        }
    }
}
