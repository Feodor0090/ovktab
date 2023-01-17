// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.UserInterface;

namespace osu.Game.Rulesets.OvkTab.UI;

/// <summary>
/// A button with moving triangles in the background.
/// </summary>
public partial class TriangleButton : OsuButton
{
    protected Triangles Triangles { get; private set; }

    [BackgroundDependencyLoader]
    private void load(OsuColour colours)
    {
        Add(Triangles = new Triangles
        {
            RelativeSizeAxes = Axes.Both,
            ColourDark = colours.BlueDarker,
            ColourLight = colours.Blue,
        });
    }
}
