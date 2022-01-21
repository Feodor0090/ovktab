using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.IO.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace osu.Game.Rulesets.OvkTab
{
    [Cached]
    public partial class OvkApiHub
    {

        public readonly VkApi api;

        public readonly Bindable<SimpleVkUser> loggedUser = new();

        /// <summary>
        /// Will be true, if longpoll is failing.
        /// </summary>
        public readonly BindableBool isLongpollFailing = new(false);

        public string Token
        {
            get
            {
                return api.Token;
            }
        }

        public int UserId
        {
            get
            {
                return (int)(api.UserId ?? 0);
            }
        }

        public OvkApiHub()
        {
            api = new VkApi();
        }
        const int IPhoneId = 3140623;
        const string IPhoneSecret = "VeWdmVclDCtn6ihuP1nt";
        const int AndroidId = 2274003;
        const string AndroidSecret = "hHbZxrka2uZ6jB1inYsH";
        const int VkMeId = 6146827;
        const string VkMeSecret = "qVxWRF1CwHERuIrKBnqe";
        const int VkmId = 2685278;
        const string VkmSecret = "lxhD8OD7dMsqtXIm5IUY";

        public void Auth(string login, string password)
        {
            string result;
            using (WebRequest request = new()
            {
                Method = HttpMethod.Get,
                Url = "https://oauth.vk.com/token?grant_type=password" +
                "&client_id=" + VkmId + "&client_secret=" + VkmSecret + "&username=" + login + "&password=" + password +
                "&scope=notify,friends,photos,audio,video,docs,notes,pages,status,offers,questions,wall,groups,messages,notifications,stats,ads"
            })
            {
                request.Perform();
                result = request.GetResponseString();
            }
            if (result[0] != '{')
                throw new Exception(result);

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

            Auth(int.Parse(dict["user_id"]), dict["access_token"]);
        }

        public void Auth(int id, string token)
        {
            api.Authorize(new ApiAuthParams { AccessToken = token, UserId = id });
            loggedUser.Value = new SimpleVkUser(api.Users.Get(new[] { (long)id }, ProfileFields.All, NameCase.Nom).First());
            StartLongPoll();
            loggedUser.ValueChanged += e =>
             {
                 if (e.NewValue == null) StopLongPoll();
             };
        }

        public static IEnumerable<(NewsItem, SimpleVkUser)> ProcessNewsFeed(NewsFeed feed)
        {
            List<(NewsItem, SimpleVkUser)> result = new();
            foreach (var post in feed.Items)
            {
                SimpleVkUser user;
                if (post.SourceId < 0)
                {
                    user = feed.Groups.Where(x => x.Id == -post.SourceId).Select(g => new SimpleVkUser()
                    {
                        id = (int)post.SourceId,
                        name = g.Name,
                        avatarUrl = g.Photo50.AbsoluteUri
                    }).First();
                }
                else
                {
                    user = feed.Profiles.Where(x => x.Id == post.SourceId).Select(x => new SimpleVkUser()
                    {
                        id = (int)post.SourceId,
                        name = x.FirstName + " " + x.LastName,
                        avatarUrl = x.Photo50.AbsoluteUri,
                        full = x,
                    }).First();
                }

                result.Add((post, user));
            }

            return result;
        }
        public async Task<IEnumerable<(NewsItem, SimpleVkUser)>> LoadNews()
        {
            NewsFeed feed = await api.NewsFeed.GetAsync(new NewsFeedGetParams() { Filters = NewsTypes.Post, MaxPhotos = 10 });
            return ProcessNewsFeed(feed);
        }
        public async Task<IEnumerable<(NewsItem, SimpleVkUser)>> LoadRecommended()
        {
            NewsFeed feed = await api.NewsFeed.GetRecommendedAsync(new NewsFeedGetRecommendedParams { MaxPhotos = 10 });
            return ProcessNewsFeed(feed);
        }

        public async Task<IEnumerable<(Post, SimpleVkUser)>> LoadWall(int pageId)
        {
            var r = await api.Wall.GetAsync(new WallGetParams { Count = 100, Extended = true, OwnerId = pageId });
            List<(Post, SimpleVkUser)> result = new();
            foreach (var post in r.WallPosts)
            {
                SimpleVkUser user;
                if (post.FromId < 0)
                {
                    user = r.Groups.Where(x => x.Id == -post.FromId).Select(g => new SimpleVkUser()
                    {
                        id = (int)post.FromId,
                        name = g.Name,
                        avatarUrl = g.Photo50.AbsoluteUri
                    }).First();
                }
                else
                {
                    user = r.Profiles.Where(x => x.Id == post.FromId).Select(x => new SimpleVkUser()
                    {
                        id = (int)post.FromId,
                        name = x.FirstName + " " + x.LastName,
                        avatarUrl = x.Photo50.AbsoluteUri,
                        full = x,
                    }).First();
                }

                result.Add((post, user));
            }

            return result;
        }

        public async Task<long?> LikePost(int ownerId, int postId, bool add)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if (add)
                        return api.Likes.Add(new LikesAddParams() { Type = LikeObjectType.Post, OwnerId = ownerId, ItemId = postId });
                    else
                        return api.Likes.Delete(LikeObjectType.Post, postId, ownerId);
                });
            }
            catch
            {
                return null;
            }
        }

        public async Task<long?> LikeComment(Comment comm)
        {
            try
            {
                return await Task.Run(() =>
                {
                    return api.Likes.Add(new LikesAddParams() { Type = LikeObjectType.Comment, OwnerId = comm.OwnerId, ItemId = comm.Id });
                });
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<SimpleVkUser>> GetFriendsList()
        {
            var f = await api.Friends.GetAsync(new FriendsGetParams()
            {
                Count = 100,
                UserId = api.UserId.Value,
                Fields = ProfileFields.All,
            });
            return f.Select(x => new SimpleVkUser(x));
        }

        public async Task<IEnumerable<SimpleVkUser>> GetGroupsList()
        {
            var g = await api.Groups.GetAsync(new GroupsGetParams() { Count = 100, Extended = true, UserId = api.UserId.Value, Fields = GroupsFields.All });
            return g.Select(x => new SimpleVkUser(x));
        }

        public async Task<IEnumerable<(SimpleVkUser, ConversationAndLastMessage)>> GetDialogsList()
        {
            var d = await api.Messages.GetConversationsAsync(new GetConversationsParams { Count = 100, Extended = true });
            return MapObjectsWithUsers(d.Items, await Convert(d.Profiles, d.Groups), x => (int)x.Conversation.Peer.Id);
        }

        public async Task<(int?, int?)?> Repost(int ownerId, int postId)
        {
            try
            {
                var p = await api.Wall.RepostAsync("wall" + ownerId + "_" + postId, String.Empty, null, false);
                return (p.LikesCount, p.RepostsCount);
            }
            catch
            {
                return (null, null);
            }
        }

        public async Task<bool> AddToBookmarks(int ownerId, int postId)
        {
            return await api.Fave.AddPostAsync(new VkNet.Model.RequestParams.Fave.FaveAddPostParams { Id = postId, OwnerId = ownerId });
        }

        public async Task<(IEnumerable<(SimpleVkUser, Message)>, SimpleVkUser[])> LoadHistory(long peer)
        {
            var m = await api.Messages.GetHistoryAsync(new MessagesGetHistoryParams { Count = 200, Extended = true, PeerId = peer });
            var u = await Convert(m.Users, m.Groups);
            return (MapObjectsWithUsers(m.Messages, u, x => (int)x.FromId), u);
        }
        public static async Task<SimpleVkUser[]> Convert(IEnumerable<User> people, IEnumerable<Group> groups)
        {
            var r = await Task.Run(() =>
            {
                IEnumerable<SimpleVkUser> peopleConverted = people?.Select(x => new SimpleVkUser(x)) ?? Array.Empty<SimpleVkUser>();
                IEnumerable<SimpleVkUser> groupsConverted = groups?.Select(x => new SimpleVkUser(x)) ?? Array.Empty<SimpleVkUser>();
                return peopleConverted.Concat(groupsConverted).ToArray();
            });
            return r;
        }
        public static (SimpleVkUser, T)[] MapObjectsWithUsers<T>(IEnumerable<T> input, IEnumerable<SimpleVkUser> users, Func<T, int> idGetter)
        {
            var result = input.Select(x => (users.Where(u => u.id == idGetter(x)).FirstOrDefault(), x)).ToArray();
            return result;
        }

        public async Task<(IEnumerable<Comment>, SimpleVkUser[], int, bool, bool)> GetComments(int ownerId, int postId)
        {
            var r = await api.Wall.GetCommentsAsync(new WallGetCommentsParams
            {
                Count = 100,
                Extended = true,
                NeedLikes = true,
                PreviewLength = 0,
                Sort = VkNet.Enums.SortOrderBy.Asc,
                PostId = postId,
                OwnerId = ownerId,
                ThreadItemsCount = 10,
            });
            var u = await Convert(r.Profiles, r.Groups);
            return (r.Items, u, (int)r.Count, r.CanPost == true, r.CanLike != false);
        }

        public async Task WriteComment(int ownerId, int postId, int replyTo, string text)
        {
            await api.Wall.CreateCommentAsync(new WallCreateCommentParams
            {
                OwnerId = ownerId,
                PostId = postId,
                ReplyToComment = replyTo,
                Message = text
            });
        }

        public void Logout()
        {
            api.LogOut();
            loggedUser.Value = null;
        }

        public async Task<bool> SendMessage(int peerId, string text)
        {
            try
            {
                await api.Messages.SendAsync(new MessagesSendParams
                {
                    PeerId = peerId,
                    Message = text,
                    RandomId = DateTime.Now.Ticks
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Message LoadMessage(int id)
        {
            var r = api.Messages.GetById(new ulong[] { (ulong)id }, Array.Empty<string>(), 0, true);
            return r[0];
        }
        public async Task<bool> SendWall(int peerId, int ownerId, int postId, string text = null)
        {
            try
            {
                await api.Messages.SendAsync(new MessagesSendParams
                {
                    PeerId = peerId,
                    Message = text,
                    RandomId = DateTime.Now.Ticks,
                    Attachments = new MediaAttachment[] { new Wall() { OwnerId = ownerId, Id = postId } }
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ClearText(string text)
        {
            var r = new System.Text.RegularExpressions.Regex(@"\[\w*\|([^\]]*)\]");
            text = r.Replace(text, "$1");
            return text.Replace("$#quot;", "\"");
        }

        
        public event Action<LongpollMessage> OnNewMessage;

        private LongpollQuery currentLongPoll = null;

        public void StartLongPoll()
        {
            if (currentLongPoll != null) currentLongPoll.Dispose();
            currentLongPoll = new LongpollQuery(this);
            Task.Factory.StartNew(currentLongPoll.Run, TaskCreationOptions.LongRunning);
        }

        public void StopLongPoll()
        {
            currentLongPoll.Dispose();
            currentLongPoll = null;
        }

        public sealed class LongpollQuery : IDisposable
        {
            private readonly OvkApiHub api;

            public LongpollQuery(OvkApiHub api)
            {
                this.api = api;
            }

            private bool stop = false;
            private WebRequest request = null;

            public async void Run()
            {
                // loop for reconnection to longpoll server
                while (!stop)
                {
                    try
                    {
                        var lpp = await api.api.Messages.GetLongPollServerAsync(true);
                        api.isLongpollFailing.Value = false;
                        string activeTs = lpp.Ts;
                        // loop for handling requests
                        while (!stop)
                        {
                            LongpollData ld;
                            using (request = new()
                            {
                                Method = HttpMethod.Get,
                                Url = $"https://{lpp.Server}?act=a_check&key={lpp.Key}&ts={activeTs}&wait=25&mode=2&version=3",
                                Timeout = 60000,
                                AllowRetryOnTimeout = true
                            })
                            {

                                request.Perform();
                                request = null;
                                ld = JsonConvert.DeserializeObject<LongpollData>(request.GetResponseString());
                            }
                            activeTs = ld.Ts;
                            if (!ld.HasUpdates)
                            {
                                continue;
                            }

                            var updates = ld.GetUpdates();

                            if (stop) return;
                            foreach (LongpollUpdate update in updates)
                            {
                                if (update.type == 4)
                                {
                                    api.OnNewMessage.Invoke(new LongpollMessage(update));
                                }
                            }
                        }
                    }
                    catch
                    {
                        if (stop) return;
                        api.isLongpollFailing.Value = true;
                        await Task.Delay(10000);
                    }
                }
            }

            public void Dispose()
            {
                api.isLongpollFailing.Value = false;
                stop = true;
                request?.Abort();
            }
        }

        public sealed class LongpollData
        {
            public string Ts { get; set; }
            public object[][] updates { get; set; }

            public bool HasUpdates { get => updates != null && updates.Length > 0; }
            public LongpollUpdate[] GetUpdates()
            {
                var r = new LongpollUpdate[updates.Length];
                for (int i = 0; i < updates.Length; i++)
                {
                    r[i] = new LongpollUpdate
                    {
                        type = int.Parse(updates[i][0].ToString()),
                        data = updates[i].Skip(1).ToArray()
                    };
                }
                return r;
            }
        }

        public struct LongpollUpdate
        {
            public int type;
            public object[] data;
        }

        public struct LongpollMessage
        {
            public LongpollMessage(LongpollUpdate upd)
            {
                if (upd.type != 4) throw new ArgumentException();
                var data = upd.data;
                messageId = System.Convert.ToInt32(data[0]);
                flags = System.Convert.ToInt32(data[1]);
                targetId = System.Convert.ToInt32(data[2]);
                time = (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(System.Convert.ToInt32(data[3])).ToLocalTime();
                text = (string)data[4];
                extra = new Dictionary<string, string>();
                fromId = 0;
                foreach (var dict in data.Skip(5).Cast<JObject>())
                {
                    foreach (var p in dict)
                    {
                        if (p.Key == "from")
                            fromId = int.Parse(p.Value.ToString());
                        else
                            extra.Add(p.Key, p.Value.ToString());
                    }
                }
            }
            public int messageId;
            public int flags;
            public int targetId;
            public int fromId;
            public DateTime time;
            public string text;
            public Dictionary<string, string> extra;
        }
    }
}
