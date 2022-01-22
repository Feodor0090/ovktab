using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Online.Chat;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;
using osu.Game.Rulesets.OvkTab.UI.Components.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using VkNet.Model.Attachments;

namespace osu.Game.Rulesets.OvkTab.API
{
    public static class VkModelsExtensions
    {
        public static IEnumerable<Drawable> ParseAttachments(this IEnumerable<Attachment> atts, IReadOnlyDependencyContainer deps, IEnumerable<SimpleVkUser> users = null)
        {
            OsuGame game = deps.Get<OsuGame>();
            LargeTextureStore lts = deps.Get<LargeTextureStore>();
            BeatmapManager bm = deps.Get<BeatmapManager>();
            NotificationOverlay no = deps.Get<NotificationOverlay>();

            if (atts == null) return Array.Empty<Drawable>();
            List<Drawable> result = new();

            // photos, videos
            var photoAtts = atts.Where(x => x.Instance is Photo || x.Instance is Video || x.Instance is Document doc && doc.Preview?.Photo != null);
            var gifs = atts.Where(x => x.Instance is Document doc && doc.Preview.Photo != null);
            var photos = photoAtts
                .Select(x => x.Instance is Photo ? new ImagesRow.ImageInfo(x.Instance as Photo) : x.Instance is Video ? new ImagesRow.ImageInfo(x.Instance as Video) : new ImagesRow.ImageInfo(((Document)x.Instance).Preview.Photo, ((Document)x.Instance).Ext.ToLower() == "gif"))
                .ToList();
            if (photos.Count != 0)
            {
                if (photos.Count <= 5)
                {
                    result.Add(new ImagesRow(photos));
                }
                else if (photos.Count <= 8)
                {
                    result.Add(new ImagesRow(photos.GetRange(0, 4)));
                    result.Add(new ImagesRow(photos.Skip(4)));
                }
                else
                {
                    result.Add(new ImagesRow(photos.GetRange(0, 3)));
                    result.Add(new ImagesRow(photos.GetRange(0, 3)));
                    result.Add(new ImagesRow(photos.GetRange(0, photos.Count - 6)));
                }
            }
            List<Wall> walls = new List<Wall>();
            atts = atts.Except(photoAtts);
            foreach (var att in atts)
            {
                switch (att.Instance)
                {
                    case VkNet.Model.Attachments.Audio x:
                        string length = (x.Duration / 60).ToString().PadLeft(2, '0') + ":" + (x.Duration % 60).ToString().PadLeft(2, '0');
                        result.Add(new SimpleAttachment(FontAwesome.Solid.Music, x.Title, x.Artist, length, new[]
                        {
                            new SimpleAttachment.SpriteAttachmentAction("Icons/Hexacons/beatmap", b =>
                            {
                                game.HandleLink(new LinkDetails(LinkAction.SearchBeatmapSet, x.Artist+" "+x.Title));
                            }),
                        }));
                        break;
                    case Document x:
                        SimpleAttachment.AttachmentAction[] actions;
                        IconUsage icon;
                        // only beatmaps are supported for now.
                        if (x.Ext == "osz")
                        {
                            icon = OsuIcon.Logo;
                            actions = new[]
                            {
                                new SimpleAttachment.IconAttachmentAction(FontAwesome.Solid.Download, b =>
                                {
                                    if(b.Name == "L") return;
                                    b.Name = "L";
                                    var cc = b.BackgroundColour;
                                    b.BackgroundColour = Colour4.LimeGreen;
                                    new VkOsuFile()
                                    {
                                        docUrl = x.Uri,
                                        ext = "osz",
                                        docName = x.Title,
                                        importer = bm,
                                        OnFail = () => { b.BackgroundColour = cc; b.Name = "0"; },
                                        OnOk = () => { },
                                        PostNotification = n => no.Post(n),
                                    }.Download();
                                }),
                            };
                        }
                        else
                        {
                            icon = FontAwesome.Solid.Paperclip;
                            actions = new[]
                            {
                                new SimpleAttachment.IconAttachmentAction(FontAwesome.Solid.Link, b =>
                                {
                                    game.HandleLink(new LinkDetails(LinkAction.External, x.Uri));
                                }),
                            };
                        }
                        result.Add(new SimpleAttachment(icon, x.Title, $"{x.Size / 1024} kb", null, actions));
                        break;
                    case AudioMessage x:
                        string len = (x.Duration / 60).ToString().PadLeft(2, '0') + ":" + (x.Duration % 60).ToString().PadLeft(2, '0');
                        result.Add(new SimpleAttachment(FontAwesome.Solid.Microphone, "Voice message", len, null));
                        if (x.Transcript != null) result.Add(new TextFlowContainer
                        {
                            Text = x.Transcript,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Padding = new() { Horizontal = 5 }
                        });
                        break;
                    case VkNet.Model.Attachments.Link x:
                        result.Add(new SimpleAttachment(FontAwesome.Solid.ExternalLinkAlt, x.Title, x.Uri.AbsoluteUri, null, new[]
                        {
                            new SimpleAttachment.IconAttachmentAction(FontAwesome.Solid.Link, b =>
                            {
                                game.HandleLink(new LinkDetails(LinkAction.External, x.Uri.AbsoluteUri));
                            }),
                        }));
                        break;
                    case Sticker x:
                        result.Add(new Sprite
                        {
                            Width = 150,
                            Height = 150,
                            Texture = lts?.Get(x.Images.Skip(1).First().Url.AbsoluteUri)
                        });
                        break;
                    case Poll x:
                        result.Add(new SimpleAttachment(FontAwesome.Solid.Poll, x.Question, $"{x.Votes ?? 0} users voted", null, null));
                        break;
                    case Wall x:
                        walls.Add(x);
                        break;
                    default:
                        result.Add(new SimpleAttachment(FontAwesome.Solid.Ban, "Unsupported attachment", att.Instance.ToString(), null, null));
                        break;
                }
            }

            foreach(var x in walls)
            {
                result.Add(new DrawableVkWall(x, users?.Where(u=>u.id == x.OwnerId).FirstOrDefault()));
            }
            return result;
        }

        public static string ClearTextFromMentions(this string text)
        {
            var r = new System.Text.RegularExpressions.Regex(@"\[\w*\|([^\]]*)\]");
            text = r.Replace(text, "$1");
            return text.Replace("$#quot;", "\"");
        }
    }
}
