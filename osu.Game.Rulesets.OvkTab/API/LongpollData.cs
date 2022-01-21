using System.Linq;

namespace osu.Game.Rulesets.OvkTab.API
{
    public sealed class LongpollData
    {
        public string Ts { get; set; }
        public object[][] updates { get; set; }

        public bool HasUpdates { get => updates != null && updates.Length > 0; }
        public LongpollUpdate[] GetUpdates()
        {
            var r = new LongpollUpdate[updates.Length];
            for (int i = 0; i < updates.Length; i++)
            {
                r[i] = new LongpollUpdate
                {
                    type = int.Parse(updates[i][0].ToString()),
                    data = updates[i].Skip(1).ToArray()
                };
            }
            return r;
        }
    }
}
