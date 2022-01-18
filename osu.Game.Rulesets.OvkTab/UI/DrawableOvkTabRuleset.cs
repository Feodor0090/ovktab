// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.OvkTab.Objects;
using osu.Game.Rulesets.OvkTab.Objects.Drawables;
using osu.Game.Rulesets.OvkTab.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.OvkTab.UI
{
    [Cached]
    public class DrawableOvkTabRuleset : DrawableRuleset<OvkTabHitObject>
    {
        public DrawableOvkTabRuleset(OvkTabRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override Playfield CreatePlayfield() => new OvkTabPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new OvkTabFramedReplayInputHandler(replay);

        public override DrawableHitObject<OvkTabHitObject> CreateDrawableRepresentation(OvkTabHitObject h) => new DrawableOvkTabHitObject(h);

        protected override PassThroughInputManager CreateInputManager() => new OvkTabInputManager(Ruleset?.RulesetInfo);
    }
}
