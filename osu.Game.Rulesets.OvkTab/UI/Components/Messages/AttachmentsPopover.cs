using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.Misc;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Messages
{
    public class AttachmentsPopover : OsuPopover
    {

        [Resolved]
        IBindable<WorkingBeatmap> wb { get; set; }
        [Resolved]
        IOvkApiHub api { get; set; }
        public AttachmentsPopover(DialogsTab tab)
        {
            Drawable reply;
            var font = OsuFont.GetFont(size: 20f);
            if (tab.replyMessage.Value == 0)
                reply = new TextFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    TextAnchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Text = "No message selected as reply\nclick any to select!"
                };
            else
                reply = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[] {
                            new OsuSpriteText
                            {
                                Text = "Reply:",
                                Position = new(5, 17.5f),
                                Origin = Anchor.CentreLeft,
                                Font = font,
                            },
                            new Container
                            {
                                Padding = new MarginPadding{ Left = 5, Right = 50 },
                                RelativeSizeAxes = Axes.Both,
                                Child = new OsuSpriteText
                                {
                                    Text = tab.replyPreview.Value,
                                    Position = new(0, 42.5f),
                                    RelativeSizeAxes = Axes.X,
                                    Truncate = true,
                                    Font = font,
                                    Origin = Anchor.CentreLeft,
                                },
                            },
                            new IconTrianglesButton
                            {
                                Size = new(40, 50),
                                icon = FontAwesome.Solid.TrashAlt,
                                iconSize = new(30),
                                Origin = Anchor.TopRight,
                                Anchor = Anchor.TopRight,
                                Position = new(-5,5),
                                Action = () =>
                                {
                                    tab.replyMessage.Value = 0;
                                    this.HidePopover();
                                }
                            }
                        }
                };

            Add(new FillFlowContainer
            {
                Spacing = new(0, 5),
                Width = 350,
                AutoSizeAxes = Axes.Y,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 60,
                        Masking = true,
                        CornerRadius = 10,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0.3f,
                                Colour = Colour4.Black
                            },
                            new KiaiTriangles(Colour4.Gray, Colour4.CadetBlue, 0.5f),
                            reply
                        }
                    },
                    new TriangleButton
                    {
                        Text = "Send with link to now playing song",
                        RelativeSizeAxes = Axes.X,
                        Height = 40,
                        Action = () =>
                        {
                            int peer = tab.currentChat.Value;
                            if(peer == 0) return;
                            try {
                                var title = wb.Value.BeatmapSetInfo.ToString();
                                var link = $"https://osu.ppy.sh/beatmapsets/{wb.Value.BeatmapSetInfo.OnlineID}/";

                                api.SendLink(peer, title, link,$"{tab.TypedText} \n\nNow playing \"{title}\", {link}", tab.replyMessage.Value);
                                tab.TypedText = string.Empty;
                                tab.replyMessage.Value = 0;
                                this.HidePopover();
                            } catch {
                            }
                        }
                    },
                    new TriangleButton
                    {
                        Text = "More options coming soon",
                        RelativeSizeAxes = Axes.X,
                        Height = 40
                    },
                }
            });
        }
    }
}
