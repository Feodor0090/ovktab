// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements;

public partial class LikeButton : PostActionButton
{
    public LikeButton()
        : base(FontAwesome.Regular.Heart, true)
    {
        TooltipText = "like";
    }
}
