// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Replays;
using osuTK;

namespace osu.Game.Rulesets.OvkTab.Replays
{
    public class OvkTabReplayFrame : ReplayFrame
    {
        public List<OvkTabAction> Actions = new();
        public Vector2 Position;

        public OvkTabReplayFrame(OvkTabAction? button = null)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }
    }
}
