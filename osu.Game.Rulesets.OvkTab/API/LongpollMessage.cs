using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Game.Rulesets.OvkTab.API
{
    public struct LongpollMessage
    {
        public LongpollMessage(LongpollUpdate upd)
        {
            if (upd.type != 4) throw new ArgumentException();
            var data = upd.data;
            messageId = Convert.ToInt32(data[0]);
            flags = Convert.ToInt32(data[1]);
            targetId = Convert.ToInt32(data[2]);
            time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt32(data[3])).ToLocalTime();
            text = (string)data[4];
            extra = new Dictionary<string, string>();
            fromId = 0;
            foreach (var dict in data.Skip(5).Cast<JObject>())
            {
                foreach (var p in dict)
                {
                    if (p.Key == "from")
                        fromId = int.Parse(p.Value.ToString());
                    else
                        extra.Add(p.Key, p.Value.ToString());
                }
            }
        }
        public int messageId;
        public int flags;
        public int targetId;
        public int fromId;
        public DateTime time;
        public string text;
        public Dictionary<string, string> extra;
    }
}
