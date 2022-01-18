using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class SimpleAttachment : BeatSyncedContainer
    {
        private Circle circle;

        Colour4 kiai = Colour4.PaleVioletRed;
        Colour4 normal = Colour4.BlueViolet.Lighten(0.05f);
        private (object, Action<TriangleButton>)[] buttons;
        string trackName;

        public SimpleAttachment(IconUsage icon, string firstLine, string secondLine, string note, (object, Action<TriangleButton>)[] buttons = null)
        {
            if (buttons == null) buttons = Array.Empty<(object, Action<TriangleButton>)>();
            trackName = secondLine + " " + firstLine;
            this.buttons = buttons;
            RelativeSizeAxes = Axes.X;
            Height = 50;
            var font = OsuFont.GetFont(size: 20);
            Padding = new MarginPadding { Horizontal = 5 };
            Children = new Drawable[]
            {
                circle = new Circle
                {
                    Size = new(50),
                    Colour = Colour4.BlueViolet
                },
                new SpriteIcon
                {
                    Position = new(25),
                    Origin = Anchor.Centre,
                    Size = new(20),
                    Icon = icon
                },
                new OsuSpriteText()
                {
                    Text = firstLine,
                    Position = new(60, 50f/4),
                    Origin = Anchor.CentreLeft,
                    Font = font,
                },
                new OsuSpriteText()
                {
                    Text = secondLine,
                    Position = new(60, 50f*3f/4f),
                    Origin = Anchor.CentreLeft,
                    Font = font,
                },
                new OsuSpriteText()
                {
                    Text = note,
                    Position = new(-45*buttons.Length, 50f/4),
                    Origin = Anchor.CentreRight,
                    Anchor = Anchor.TopRight,
                    Font = font,
                },
            };
            for (int i = 0; i < buttons.Length; i++)
            {
                Add(new TriangleButton
                {
                    Size = new(40, 50),
                    Position = new(-45 * i, 0),
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                });
            }
        }

        [BackgroundDependencyLoader]
        void load(TextureStore ts)
        {
            var btns = this.OfType<TriangleButton>().ToArray();
            for (int i = 0; i < btns.Length; i++)
            {
                var button = btns[i];
                if (buttons[i].Item1 is IconUsage icon)
                {
                    button.Add(new SpriteIcon
                    {
                        Icon = icon,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new(20),
                    });
                }
                else
                {
                    button.Add(new Sprite
                    {
                        Texture = ts?.Get(buttons[i].Item1.ToString()),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new(30),
                    });
                }
                button.Action = () => buttons[i].Item2(button);
            }
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            circle?.FlashColour(effectPoint.KiaiMode ? kiai : normal, timingPoint.BeatLength);
        }
    }
}
