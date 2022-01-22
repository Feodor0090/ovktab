using System.Linq;

namespace osu.Game.Rulesets.OvkTab.API
{
    public sealed class LongpollData
    {
        public string Ts { get; set; }
        public object[][] Updates { get; set; }

        public bool HasUpdates { get => Updates != null && Updates.Length > 0; }
        public LongpollUpdate[] GetUpdates()
        {
            var r = new LongpollUpdate[Updates.Length];
            for (int i = 0; i < Updates.Length; i++)
            {
                r[i] = new LongpollUpdate
                {
                    type = int.Parse(Updates[i][0].ToString()),
                    data = Updates[i].Skip(1).ToArray()
                };
            }
            return r;
        }
    }
}
