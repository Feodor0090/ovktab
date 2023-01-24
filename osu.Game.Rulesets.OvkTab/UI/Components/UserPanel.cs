using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.OvkTab.API;
using osuTK;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public partial class UserPanel : BeatSyncedContainer, IHasTooltip
    {
        private readonly SimpleVkUser user;
        private Box bg = null!;

        public UserPanel(SimpleVkUser user)
        {
            RelativeSizeAxes = Axes.X;
            Height = 60;
            this.user = user;
            Masking = true;
            CornerRadius = 15;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colour)
        {
            kiai = colour.PinkLight;
            Add(bg = new Box
            {
                Colour = Colour4.White,
                Alpha = 0.1f,
                RelativeSizeAxes = Axes.Both,
            });

            OsuSpriteText t;
            Add(t = new OsuSpriteText
            {
                Text = user.name,
                Position = new Vector2(65, 0),
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Font = OsuFont.GetFont(size: 24),
            });

            if (user.full is User u)
            {
                if (u.Online ?? false)
                    t.Colour = colour.Lime;
            }

            LoadComponentAsync(new DrawableVkAvatar(user)
            {
                Position = new(30)
            }, Add);
        }

        private Colour4 kiai;
        private readonly Colour4 normal = Colour4.FromHex("EEE");

        protected override bool OnHover(HoverEvent e)
        {
            bg.FadeTo(0.2f, 200, Easing.OutQuint);
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            bg.FadeTo(0.1f, 500);
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);
            bg.FlashColour(effectPoint.KiaiMode ? kiai : normal, timingPoint.BeatLength);
        }

        public LocalisableString TooltipText => "click the avatar to browse the wall";
    }
}
