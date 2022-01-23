using System;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.API
{
    public struct LongpollMessageEdit
    {
        public LongpollMessageEdit(LongpollUpdate upd)
        {
            if (upd.type != 5) throw new ArgumentException();
            var data = upd.data;
            messageId = Convert.ToInt32(data[0]);
            peerId = Convert.ToInt32(data[2]);
        }

        public int messageId;
        public int peerId;

        public Message LoadFull(IOvkApiHub api)
        {
            return api.LoadMessage(messageId);
        }
    }
}
