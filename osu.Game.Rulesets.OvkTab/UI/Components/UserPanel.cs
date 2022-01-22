using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.OvkTab.API;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class UserPanel : BeatSyncedContainer
    {
        SimpleVkUser user;
        private Box bg;

        public UserPanel(SimpleVkUser user)
        {
            RelativeSizeAxes = Axes.X;
            Height = 60;
            this.user = user;
            Masking = true;
            CornerRadius = 15;
        }

        [BackgroundDependencyLoader]
        void load(OsuColour colour)
        {
            Add(bg = new Box()
            {
                Colour = Colour4.Black,
                Alpha = 0.3f,
                RelativeSizeAxes = Axes.Both,
            });
            LoadComponentAsync(new DrawableVkAvatar(user)
            {
                Position = new(30)
            }, x =>
            {
                Add(x);
            });
            OsuSpriteText t;
            Add(t = new OsuSpriteText()
            {
                Text = user.name,
                Position = new(65, 0),
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Font = OsuFont.GetFont(size: 24),
            });
            if (user.full is User u)
            {
                if (u.Online ?? false)
                    t.Colour = colour.Lime;
            };

        }
        Colour4 kiai = Colour4.FromHex("222");
        Colour4 normal = Colour4.FromHex("111");
        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            bg?.FlashColour(effectPoint.KiaiMode ? kiai : normal, timingPoint.BeatLength);
        }
    }
}
