using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.OvkTab.API;
using System;
using System.Collections.Generic;
using VkNet.Model.Attachments;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    public abstract class DrawableVkMessage : Container
    {
        protected SimpleVkUser user;
        public FillFlowContainer content;
        public FillFlowContainer header;
        public DrawableVkMessage(SimpleVkUser author)
        {
            user = author;

            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            var font = OsuFont.GetFont(size: 20);

            header = new FillFlowContainer
            {
                Direction = FillDirection.Horizontal,
                Spacing = new(10, 0),
                Height = 20,
                RelativeSizeAxes = Axes.X,
                Padding = new() { Left = 65 }
            };
            Add(header);
            if (user != null) header.Add(new OsuSpriteText
            {
                Text = user.name,
                Font = font,
            });

            Add(new Container
            {
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
                Padding = new MarginPadding { Right = 5, Bottom = 5, Left = 60, Top = 20 },
                Child = new Container
                {
                    Masking = true,
                    CornerRadius = 10,
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0.3f,
                            Colour = Colour4.Black
                        },
                        content = new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Y,
                            RelativeSizeAxes = Axes.X,
                            Padding = new MarginPadding(5),
                            Spacing = new(0, 10),
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Initializes this message with text, send date and list of attachments.
        /// </summary>
        /// <param name="text">Text of the message.</param>
        /// <param name="atts">List of attachments</param>
        /// <param name="date">Time, when this message was sent.</param>
        protected void AddContent(string text, IEnumerable<Attachment> atts, DateTime date, IEnumerable<SimpleVkUser> users = null)
        {
            content.Add(new TextFlowContainer(x => x.Font = OsuFont.GetFont(size: 18))
            {
                Text = text.ClearTextFromMentions(),
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new() { Horizontal = 5 },
            });
            content.AddRange(atts.ParseAttachments(Dependencies, users));
            header.Add(new OsuSpriteText
            {
                Font = OsuFont.GetFont(size: 18),
                Text = $"{date:d MMMM HH:mm}",
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Colour = Colour4.Gray
            });
            LoadComponentAsync(new DrawableVkAvatar(user)
            {
                Position = new(30, 25),
            }, Add);
        }
    }
}
