using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.OvkTab.UI.Components;
using osu.Framework.Graphics;
using osu.Game.Tests.Visual;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;

namespace osu.Game.Rulesets.OvkTab.Tests.UI
{
    public class TestSceneAttachments : OsuTestScene
    {
        public static Drawable[] Generate()
        {
            var imgs = new ImagesRow.ImageInfo[][]
            {
                new ImagesRow.ImageInfo[]
                {
                    new()
                    {
                        normal = "https://sun9-33.userapi.com/impg/023sAaADqSVeonlyoYl-gwiQ_PdhVBK1tXVmNw/4bcf2YZgGUg.jpg?size=940x553&quality=96&sign=d3591a52efc633b19445fabf02178105&type=album",
                        w = 940,
                        h = 553,
                        length = 292*25,
                    },
                    new()
                    {
                        normal = "https://sun9-68.userapi.com/impf/c845219/v845219644/14360/L76Iqetftc8.jpg?size=856x922&quality=96&sign=ce5d8774719a9cba83923047967589cb&type=album",
                        w = 856,
                        h = 922,
                        length = -1,
                    },
                    new()
                    {
                        normal = "https://sun9-38.userapi.com/impg/PvMGlvUOg8fRz6HrUtALOYYaQPSD3iCKoVGC4A/8UlHJ7l_jsA.jpg?size=1080x883&quality=96&sign=c5cb1374260323ac1aedc86a1d6428f3&type=album",
                        w = 1080,
                        h = 883,
                        length = -1,
                    },
                    new()
                    {
                        normal="https://sun9-83.userapi.com/impg/iJYe-edB3znwDPBDRIuqkRtZTQan8FrnI1-TSQ/imYVnzV5LfA.jpg?size=1280x800&quality=96&sign=0a18343ea5e7a42b9fb28aecbd51d46a&type=album",
                        w = 1280,
                        h = 800,
                        length = -1,
                    }
                },
                new ImagesRow.ImageInfo[]
                {
                    new()
                    {
                        normal = "https://sun9-11.userapi.com/impg/gZw3nBH1o-RP173dwfNIOIvftTnkdE6uhDT4mw/7Be0MvsfOrA.jpg?size=1080x770&quality=96&sign=412dde9f8ad3f64c22ecaf7507078e11&type=album",
                        w = 1080,
                        h = 770,
                        length = 7*60+27
                    },
                    new()
                    {
                        normal = "https://sun9-5.userapi.com/impf/28GU_521NrGtRx8WlW0tuCn5jFkWyah5kHmT9g/X1BBxjFI2YE.jpg?size=748x772&quality=96&sign=300eef92b5a667e706c7819f7e1e4beb&type=album",
                        w = 748,
                        h = 772,
                        length = -1,
                    },
                    new()
                    {
                        normal = "https://sun9-14.userapi.com/impg/Qr8HygfMXjZPmU9cDm0K2pr8I-LjmZkTc4XRqQ/HmYd3mFDxrs.jpg?size=1580x1020&quality=95&sign=5cbd41a60c4ee0b2fceafe3773ea6fc7&type=album",
                        w = 1580,
                        h = 1020,
                        length = 0,
                    }
                }
            };
            var audious = (new (string, string)[]
            {
                ("Ochame Kinou", "Various Artists"),
                ("Запись-0016", "ИИИГОРЬ"),
                ("Vishnevaya devyatka", "Leonid Radnevsky")
            }).Select(x => new SimpleAttachment(FontAwesome.Solid.Music, x.Item1, x.Item2, "7:27"));

            return imgs.Select(x => new ImagesRow(x)).Cast<Drawable>().Concat(audious).ToArray();
        }
        protected override void LoadComplete()
        {
            base.LoadComplete();
            AddStep("Add row", () =>
            {
                Clear(true);

                Add(new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new osuTK.Vector2(0.8f, 1f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new(0, 10),
                    Children = Generate()
                });
            });
        }
    }
}
