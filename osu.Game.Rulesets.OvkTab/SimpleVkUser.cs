using VkNet.Model;

namespace osu.Game.Rulesets.OvkTab
{
    public class SimpleVkUser
    {
        public SimpleVkUser()
        {
        }

        public SimpleVkUser(User u)
        {
            full = u;
            id = (int)u.Id;
            name = u.FirstName + " " + u.LastName;
            avatarUrl = u.Photo50?.AbsoluteUri;
        }
        public SimpleVkUser(Group u)
        {
            full = u;
            id = -(int)u.Id;
            name = u.Name;
            avatarUrl = u.Photo50?.AbsoluteUri;
        }
        public int id;
        public string name;
        public string avatarUrl;
        public object full;
    }
}
