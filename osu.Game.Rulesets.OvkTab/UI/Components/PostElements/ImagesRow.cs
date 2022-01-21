using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Cursor;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;
using osu.Framework.Graphics.Sprites;
using VkNet;
using VkNet.Model;
using VkNet.Model.Attachments;
using osu.Framework.Graphics.Textures;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Rulesets.OvkTab.UI.Components.PostElements
{
    public class ImagesRow : FillFlowContainer
    {
        ImageInfo[] images;
        int loadedCount = 0;

        public ImagesRow(IEnumerable<ImageInfo> photos)
        {
            RelativeSizeAxes = Axes.X;
            images = photos.ToArray();
            Height = 200;
            LoadingLayer ll = new(dimBackground: true)
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 20,
            };
            Add(ll);
            ll.Show();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Task.Run(async () =>
            {
                await Task.Delay(200);
                var items = new ImageAttachment[images.Length];
                int maxH = images.Max(i => i.h);
                if (items.Length == 1 && maxH > 350) maxH = 350;
                float totalW = 0;
                for (int i = 0; i < images.Length; i++)
                {
                    float sw = images[i].w * ((float)maxH / images[i].h);
                    totalW += sw;
                }
                float avalW = DrawWidth - 10 * images.Length; // margins
                float mul = avalW / totalW;
                if (images.Length == 1 && mul > 1) mul = 1;
                for (int i = 0; i < images.Length; i++)
                {
                    float sw = images[i].w * ((float)maxH / images[i].h);
                    items[i] = new ImageAttachment(images[i].normal, sw * mul, images[i].length);
                    items[i].Hide();
                    items[i].OnLoadComplete += OnItemLoaded;
                }
                Schedule(() =>
                {
                    Height = maxH * mul;
                    AddRange(items);
                });
            });
        }

        void OnItemLoaded(Drawable d)
        {
            loadedCount++;
            if (loadedCount == images.Length)
                Schedule(() =>
                {
                    var ll = Children.OfType<LoadingLayer>().First();
                    Remove(ll);
                    ll.Dispose();
                    foreach (var d in this) d.Show();
                });
        }

        public struct ImageInfo
        {
            public int w; public int h;
            public string normal;
            public int length;

            public ImageInfo(Photo x, bool isGif = false)
            {
                PhotoSize size = x.Sizes.Where(p => p.Width < 1280).OrderByDescending(p => p.Height).First();
                normal = size.Url?.AbsoluteUri ?? size.Src.AbsoluteUri;
                w = (int)size.Width;
                h = (int)size.Height;
                length = isGif ? -2 : -1;
            }
            public ImageInfo(Video x)
            {
                VideoImage size = x.Image.Where(p => p.Width < 1280).OrderByDescending(p => p.Height).First();
                normal = size.Url.AbsoluteUri;
                w = (int)size.Width;
                h = (int)size.Height;
                length = x.Duration ?? -1;
            }
        }
    }
}
