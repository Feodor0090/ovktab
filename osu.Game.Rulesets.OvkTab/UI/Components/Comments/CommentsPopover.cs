using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osu.Game.Rulesets.OvkTab.API;
using osu.Game.Rulesets.OvkTab.UI.Components.PostElements;
using osuTK;
using System;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab.UI.Components.Comments
{
    [Cached]
    public class CommentsPopover : OsuPopover
    {
        readonly FillFlowContainer content;
        readonly LoadingLayer ll;
        readonly OsuTextBox input;
        private readonly int ownerId;
        private readonly int postId;

        private readonly PostFooter footer;
        private IOvkApiHub ApiHub { get; set; }

        [Cached]
        public readonly PopoverContainer pc;

        public CommentsPopover(int ownerId, int postId, PostFooter f, Vector2 parentSize)
        {
            footer = f;
            this.ownerId = ownerId;
            this.postId = postId;
            Size = parentSize * 0.95f;
            Content.AutoSizeAxes = Axes.None;
            Content.RelativeSizeAxes = Axes.Both;
            Content.Children = new Drawable[]
            {
                pc = new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding
                            {
                                Bottom = 45
                            },
                            Child = new OverlayScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = content = new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y
                                }
                            }
                        },
                        input = new OsuTextBox
                        {
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            PlaceholderText = "type your comment",
                            ReleaseFocusOnCommit = false
                        },
                    }
                },
                ll = new LoadingLayer()
            };
            ll.Show();
            input.Hide();
        }

        [BackgroundDependencyLoader(true)]
        async void load(IOvkApiHub api)
        {
            if (api == null) return;
            ApiHub = api;
            input.OnCommit += Input_OnCommit;
            await Task.Run(async () =>
            {
                var c = await api.GetComments(ownerId, postId);
                var t = DrawableVkComment.BuildTree(c.Item1, c.Item2);
                bool canPost = c.Item4;
                footer.comments.Value = c.Item3;
                var a = t.Select(x => new DrawableVkComment(x, canPost, c.Item5)).ToArray();
                Schedule(() =>
                {
                    content.AddRange(a);
                    if (canPost)
                    {
                        input.Show();
                    }
                    ll.Hide();
                });
            });
        }

        private void Input_OnCommit(TextBox sender, bool newText)
        {
            SendComment(sender, sender.Text, 0, content);
            return;
        }

        public async void SendComment(TextBox sender, string text, int replyTo, FillFlowContainer container)
        {
            if (string.IsNullOrEmpty(text)) return;
            ll.Show();
            try
            {
                await ApiHub.WriteComment(ownerId, postId, replyTo, text);
                container.Add(new DrawableVkComment(new DrawableVkComment.CommentsLevel
                {
                    user = ApiHub.LoggedUser.Value,
                    replies = new(),
                    comment = new Comment
                    {
                        Text = text,
                    }
                }, false, false));
                footer.comments.Value++;
                footer.commentsButton.Checked = true;
                sender.Text = string.Empty;
            }
            catch
            {
                sender.FlashColour(Colour4.Red, 750);
            }
            ll.Hide();
        }
    }
}
