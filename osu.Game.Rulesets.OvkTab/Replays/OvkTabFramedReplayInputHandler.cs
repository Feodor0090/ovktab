// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.OvkTab.Replays
{
    public class OvkTabFramedReplayInputHandler : FramedReplayInputHandler<OvkTabReplayFrame>
    {
        public OvkTabFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }

        protected override bool IsImportant(OvkTabReplayFrame frame) => frame.actions.Any();
    }
}
