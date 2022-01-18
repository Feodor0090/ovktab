// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.OvkTab.Objects;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.OvkTab.Replays
{
    public class OvkTabAutoGenerator : AutoGenerator<OvkTabReplayFrame>
    {
        public new Beatmap<OvkTabHitObject> Beatmap => (Beatmap<OvkTabHitObject>)base.Beatmap;

        public OvkTabAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override void GenerateFrames()
        {
            Frames.Add(new OvkTabReplayFrame());

            foreach (OvkTabHitObject hitObject in Beatmap.HitObjects)
            {
                Frames.Add(new OvkTabReplayFrame
                {
                    Time = hitObject.StartTime,
                    Position = hitObject.Position,
                    // todo: add required inputs and extra frames.
                });
            }
        }
    }
}
