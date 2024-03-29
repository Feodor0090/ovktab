// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Graphics;

namespace osu.Game.Rulesets.OvkTab.UI;

public partial class DangerousTriangleButton : TriangleButton
{
    [BackgroundDependencyLoader]
    private void load(OsuColour colours)
    {
        BackgroundColour = colours.PinkDark;
        Triangles.ColourDark = colours.PinkDarker;
        Triangles.ColourLight = colours.Pink;
    }
}