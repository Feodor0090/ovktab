using osu.Framework.Allocation;
using osu.Framework.Bindables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace osu.Game.Rulesets.OvkTab.API
{
    [Cached]
    public interface IOvkApiHub
    {
        BindableBool IsLongpollFailing { get; }
        Bindable<SimpleVkUser> LoggedUser { get; }
        string Token { get; }
        int UserId { get; }

        event Action<LongpollMessage> OnNewMessage;
        event Action<LongpollMessageEdit> OnMessageEdit;

        Task<bool> AddToBookmarks(int ownerId, int postId);
        void Auth(int id, string token);
        void Auth(string login, string password);
        Task<LongPollServerResponse> ConnectToLongPoll();
        Task<bool> DeleteMessage(int peerId, ulong convMsgId, ulong id);
        Task<(IEnumerable<Comment>, SimpleVkUser[], int, bool, bool)> GetComments(int ownerId, int postId);
        Task<IEnumerable<(SimpleVkUser, ConversationAndLastMessage)>> GetDialogsList();
        Task<IEnumerable<SimpleVkUser>> GetFriendsList();
        Task<IEnumerable<SimpleVkUser>> GetGroupsList();
        Task<long?> LikeComment(Comment comm);
        Task<long?> LikePost(int ownerId, int postId, bool add);
        Task<((SimpleVkUser, Message)[], SimpleVkUser[])> LoadHistory(long peer);
        Message LoadMessage(int id);
        Task<IEnumerable<(NewsItem, SimpleVkUser)>> LoadNews();
        Task<IEnumerable<(NewsItem, SimpleVkUser)>> LoadRecommended();
        Task<IEnumerable<(Post, SimpleVkUser)>> LoadWall(int pageId);
        void Logout();
        void ReceiveNewMessage(LongpollMessage msg);
        Task<(int?, int?)?> Repost(int ownerId, int postId);
        Task<bool> SendMessage(int peerId, string text, int replyTo);
        Task<bool> SendWall(int peerId, int ownerId, int postId, string text = null);
        void StartLongPoll();
        void StopLongPoll();
        Task WriteComment(int ownerId, int postId, int replyTo, string text);
    }
}