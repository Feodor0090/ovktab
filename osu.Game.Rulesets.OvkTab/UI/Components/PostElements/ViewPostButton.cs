// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Online.Chat;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements;

public partial class ViewPostButton : PostActionButton
{
    private readonly string link;

    public ViewPostButton(string link)
        : base(FontAwesome.Solid.Link, false)
    {
        this.link = link;
        TooltipText = "open in browser";
        Action = open;
    }

    [Resolved]
    private OsuGame? game { get; set; }

    private void open()
    {
        game?.HandleLink(new LinkDetails(LinkAction.External, link));
    }
}
