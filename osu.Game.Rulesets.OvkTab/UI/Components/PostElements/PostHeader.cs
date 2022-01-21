using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Beatmaps.ControlPoints;
using osu.Framework.Audio.Track;
using osu.Game.Rulesets.OvkTab.API;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public class PostHeader : BeatSyncedContainer
    {
        private readonly SimpleVkUser author;
        private readonly OsuSpriteText authorText;
        private DrawableVkAvatar avatar = null;

        public PostHeader(SimpleVkUser author, DateTime time)
        {
            this.author = author;
            RelativeSizeAxes = Axes.X;
            Height = 60;
            var font = OsuFont.GetFont(size: 20);
            Children = new Drawable[]
            {
                authorText = new OsuSpriteText()
                {
                    Text = author.name,
                    Position = new(65, 15),
                    Origin = Anchor.CentreLeft,
                    Font = font,
                },
                new OsuSpriteText()
                {
                    Text = $"{time:d MMMM HH:mm} ",
                    Position = new(65, 45),
                    Origin = Anchor.CentreLeft,
                    Font = font,
                }
            };
        }

        [BackgroundDependencyLoader]
        void load(LargeTextureStore lts, OsuColour colour)
        {
            LoadComponentAsync(new DrawableVkAvatar(author)
            {
                Position = new(30),
            }, x => { Add(x); avatar = x; });
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            if (effectPoint.KiaiMode)
            {
                double b4 = timingPoint.BeatLength / 4d;
                avatar?.ScaleTo(1.02f, b4 * 3, Easing.Out).Then().ScaleTo(1f, b4, Easing.OutQuint);
                authorText.FlashColour(Colour4.FromHex("BBB"), b4 * 4);
            }
        }
    }
}
