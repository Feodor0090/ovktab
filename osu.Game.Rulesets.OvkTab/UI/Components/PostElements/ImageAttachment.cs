using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    internal class ImageAttachment : Container
    {
        private readonly string url;
        private readonly int length = -1;

        public ImageAttachment(string url, float width, int length = -1)
        {
            this.length = length;
            this.url = url;
            RelativeSizeAxes = Axes.Y;
            Height = 1;
            Width = width;
            Masking = true;
            CornerRadius = 10;
            Margin = new MarginPadding { Horizontal = 5 };
        }

        [BackgroundDependencyLoader]
        void load()
        {
            LoadComponentAsync(new AttSprite(url), load);
        }

        void load(AttSprite s)
        {
            Add(s);
            if (length < 0 && length!=-2) return;
            Add(new Box
            {
                Rotation = 45,
                Origin = Anchor.Centre,
                Anchor = Anchor.TopRight,
                Colour = Colour4.Black,
                Size = new(70)
            });
            Add(new SpriteIcon
            {
                Icon = length == 0 ? FontAwesome.Solid.Video : OsuIcon.ModCinema,
                Size = new(18),
                Origin = Anchor.TopRight,
                Anchor = Anchor.TopRight,
                Position = new(-5, 5),
            });
            string text;
            if (length > 0)
            {
                text = ((length / 60).ToString().PadLeft(2, '0') + ":" + (length % 60).ToString().PadLeft(2, '0'));
            } 
            else if (length == -2)
            {
                text = "gif";
            } 
            else
            {
                text = "live";
            }
            Add(new Container
            {
                AutoSizeAxes = Axes.Both,
                Position = new(0),
                Children  = new Drawable[]
                {
                    new Box
                    {
                        Colour = Colour4.Black,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Size = new(10),
                    },
                    new Box
                    {
                        Colour = Colour4.Black,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Size = new(10),
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Position = new(0),
                        Masking = true,
                        CornerRadius = 10,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                            },
                            new Container
                            {
                                Padding = new MarginPadding (5),
                                AutoSizeAxes = Axes.Both,
                                Child = new OsuSpriteText
                                {
                                    Colour = Colour4.White,
                                    Text = text,
                                    Font = OsuFont.GetFont(size:20, weight: FontWeight.SemiBold),
                                }
                            }
                        }
                    }
                }
            });
        }



        [LongRunningLoad]
        internal class AttSprite : Sprite
        {
            private readonly string url;

            public AttSprite(string url)
            {
                this.url = url;
            }
            [BackgroundDependencyLoader]
            void load(LargeTextureStore lts)
            {
                Texture = lts.Get(url);
                RelativeSizeAxes = Axes.Both;
                Size = new(1);
                FillMode = FillMode.Fit;
                FillAspectRatio = (float)Texture.Width / Texture.Height;

            }
        }
    }
}
