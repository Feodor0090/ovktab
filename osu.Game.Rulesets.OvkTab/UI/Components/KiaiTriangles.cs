using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Containers;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public class KiaiTriangles : BeatSyncedContainer
    {
        private readonly Triangles triangles;
        public float MaxAlpha { get; private set; }
        private bool wasKiai = false;

        public KiaiTriangles(Colour4 light, Colour4 dark, float maxAlpha)
        {
            RelativeSizeAxes = Axes.Both;
            triangles = new Triangles() { ColourDark = dark, ColourLight = light, RelativeSizeAxes = Axes.Both };
            triangles.Hide();
            MaxAlpha = maxAlpha;
            Add(triangles);
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            if (effectPoint.KiaiMode && !wasKiai)
            {
                wasKiai = true;
                triangles.FadeTo(MaxAlpha, timingPoint.BeatLength, Easing.None);
            }
            if (!effectPoint.KiaiMode && wasKiai)
            {
                wasKiai = false;
                triangles.FadeOut(timingPoint.BeatLength * (int)timingPoint.TimeSignature, Easing.Out);
            }
        }
    }
}
