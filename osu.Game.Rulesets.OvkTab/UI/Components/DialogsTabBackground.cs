using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osuTK.Graphics;
using System.Threading;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Storyboards.Drawables;
using osu.Framework.Timing;
using osu.Game.Storyboards;
using osuTK;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{

    public class DialogsTabBackground : Container
    {
        private Container<StoryboardLayer> storyboardContainer;
        private LoadingSpinner loading;
        private Box bgDim;

        protected Bindable<bool> enabled;
        protected IBindable<WorkingBeatmap> Beatmap = new Bindable<WorkingBeatmap>();

        public DialogsTabBackground(Bindable<bool> enabled)
        {
            this.enabled = enabled;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Beatmap.BindValueChanged(OnBeatmapChanged, true);
            enabled.BindValueChanged(e => updateStoryboard(e.NewValue ? Beatmap.Value : null), false);
        }

        [BackgroundDependencyLoader]
        private void load(IBindable<WorkingBeatmap> working, OverlayColourProvider ovp)
        {
            Beatmap.BindTo(working);
            RelativeSizeAxes = Axes.Both;
            Children = new Drawable[]
            {
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Child = loading = new LoadingSpinner(true)
                    {
                        Scale = new Vector2(1.5f),
                    },
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(0.5f),
                    Child = storyboardContainer = new Container<StoryboardLayer>
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                    }
                },
                bgDim = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.8f,
                    Colour = ovp.Background5,
                }
            };
        }

        protected void OnBeatmapChanged(ValueChangedEvent<WorkingBeatmap> beatmap)
        {
            updateStoryboard(beatmap.NewValue);
        }

        private CancellationTokenSource cancellationToken;
        private StoryboardLayer storyboard;

        private void updateStoryboard(WorkingBeatmap beatmap)
        {
            cancellationToken?.Cancel();
            storyboard?.FadeOut(250, Easing.OutQuint).Expire();
            storyboard = null;

            if (beatmap == null || !enabled.Value)
            {
                loading.Hide();
                return;
            }

            loading.Show();

            LoadComponentAsync(new StoryboardLayer(beatmap), loaded =>
            {
                storyboardContainer.Add(storyboard = loaded);
                loaded.FadeIn(250, Easing.OutQuint);
                loading.Hide();
            }, (cancellationToken = new CancellationTokenSource()).Token);
        }

        private class StoryboardLayer : AudioContainer
        {
            private readonly WorkingBeatmap beatmap;

            public StoryboardLayer(WorkingBeatmap beatmap)
            {
                this.beatmap = beatmap;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                RelativeSizeAxes = Axes.Both;
                Volume.Value = 0;
                Alpha = 0;

                if(!beatmap.Storyboard.HasDrawable)
                {
                    Child = new BeatmapBackground(beatmap);
                    return;
                }


                Drawable layer;

                if (beatmap.Storyboard.ReplacesBackground)
                {
                    layer = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black
                    };
                }
                else
                {
                    layer = new BeatmapBackground(beatmap);
                }

                Children = new Drawable[]
                {
                    layer,
                    new FillStoryboard(beatmap.Storyboard) { Clock = new InterpolatingFramedClock(beatmap.Track) }
                };
            }

            private class FillStoryboard : DrawableStoryboard
            {
                protected override Vector2 DrawScale => Scale;

                public FillStoryboard(Storyboard storyboard)
                    : base(storyboard)
                {
                }

                protected override void Update()
                {
                    base.Update();
                    Scale = DrawWidth / DrawHeight > Parent.DrawWidth / Parent.DrawHeight ? new Vector2(Parent.DrawHeight / Height) : new Vector2(Parent.DrawWidth / Width);
                }
            }
        }
    }
}