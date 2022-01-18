using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;

namespace osu.Game.Rulesets.OvkTab.UI.Components
{
    [LongRunningLoad]
    public class DrawableVkAvatar : OsuClickableContainer, IHasPopover
    {
        SimpleVkUser user;
        readonly string url;

        [Resolved(canBeNull: true)] private PopoverContainer popoverContainer { get; set; }
        public DrawableVkAvatar(SimpleVkUser target)
        {
            user = target;
            Size = new(50);
            Origin = Anchor.Centre;
            Masking = true;
            CornerRadius = 10;
            TooltipText = user?.name;
            url = user?.avatarUrl ?? "https://vk.com/images/camera_50.png";
            Action = () => {
                this.ShowPopover();
            };
        }
        public DrawableVkAvatar(string name, string url)
        {
            Size = new(50);
            Origin = Anchor.Centre;
            Masking = true;
            CornerRadius = 10;
            TooltipText = name;
            this.url = url ?? "https://vk.com/images/camera_50.png";
        }
        [BackgroundDependencyLoader]
        void load(LargeTextureStore lts)
        {
            Add(new Sprite
            {
                Texture = lts.Get(url),
                RelativeSizeAxes = Axes.Both,
                Size = new(1)
            });
        }

        public Popover GetPopover()
        {
            if (user.id == 0) return null;
            return new WallPopover(user.id, popoverContainer?.DrawSize ?? new(600));
        }
    }
}
