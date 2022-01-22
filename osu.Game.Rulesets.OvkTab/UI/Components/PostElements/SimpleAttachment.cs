using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using System;
using System.Linq;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public class SimpleAttachment : BeatSyncedContainer
    {
        private readonly Circle circle;
        readonly Colour4 kiai = Colour4.PaleVioletRed;
        readonly Colour4 normal = Colour4.BlueViolet.Lighten(0.05f);
        private AttachmentAction[] actions;

        public SimpleAttachment(IconUsage icon, string firstLine, string secondLine, string note, AttachmentAction[] buttons = null)
        {
            actions = buttons;
            int buttonsCount = buttons?.Length ?? 0;
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
                    Position = new(-45*buttonsCount, 50f/4),
                    Origin = Anchor.CentreRight,
                    Anchor = Anchor.TopRight,
                    Font = font,
                },
            };
            for (int i = 0; i < buttonsCount; i++)
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
            if (actions == null) return;
            var btns = this.OfType<TriangleButton>().ToArray();
            for (int i = 0; i < btns.Length; i++)
            {
                var button = btns[i];
                button.Add(actions[i].Get(ts));
                button.Action = () => actions[i].Action(button);
            }
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            circle?.FlashColour(effectPoint.KiaiMode ? kiai : normal, timingPoint.BeatLength);
        }

        public abstract class AttachmentAction
        {
            public Action<TriangleButton> Action { get; protected set; }

            public abstract Drawable Get(TextureStore ts);
        }
        public class IconAttachmentAction : AttachmentAction
        {
            private readonly IconUsage icon;

            public IconAttachmentAction(IconUsage icon, Action<TriangleButton> action)
            {
                Action = action;
                this.icon = icon;
            }

            public override Drawable Get(TextureStore ts) => new SpriteIcon
            {
                Icon = icon,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new(20),
            };
        }
        public class SpriteAttachmentAction : AttachmentAction
        {
            private readonly string sprite;

            public SpriteAttachmentAction(string texName, Action<TriangleButton> action)
            {
                sprite = texName;
                Action = action;
            }

            public override Drawable Get(TextureStore ts) => new Sprite
            {
                Texture = ts?.Get(sprite),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new(30),
            };
        }
    }
}
