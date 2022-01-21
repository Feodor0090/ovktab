using osu.Framework.Bindables;
using osu.Game.Rulesets.OvkTab.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace osu.Game.Rulesets.OvkTab.Tests.Stubs
{
    internal class OvkApiHubStub : IOvkApiHub
    {
        public BindableBool IsLongpollFailing => throw new NotImplementedException();

        public Bindable<SimpleVkUser> LoggedUser => throw new NotImplementedException();

        public string Token => "Maho-sempai";

        public int UserId => 727;

        public event Action<LongpollMessage> OnNewMessage;

        public Task<bool> AddToBookmarks(int ownerId, int postId)
        {
            return Task.FromResult(true);
        }

        public void Auth(int id, string token)
        {
            throw new NotImplementedException();
        }

        public void Auth(string login, string password)
        {
            throw new NotImplementedException();
        }

        public Task<LongPollServerResponse> ConnectToLongPoll()
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<Comment>, SimpleVkUser[], int, bool, bool)> GetComments(int ownerId, int postId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(SimpleVkUser, ConversationAndLastMessage)>> GetDialogsList()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SimpleVkUser>> GetFriendsList()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SimpleVkUser>> GetGroupsList()
        {
            throw new NotImplementedException();
        }

        public Task<long?> LikeComment(Comment comm)
        {
            return Task.FromResult<long?>(72);
        }

        public Task<long?> LikePost(int ownerId, int postId, bool add)
        {
            return Task.FromResult<long?>(add ? 728 : 726);
        }

        public Task<(IEnumerable<(SimpleVkUser, Message)>, SimpleVkUser[])> LoadHistory(long peer)
        {
            throw new NotImplementedException();
        }

        public Message LoadMessage(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(NewsItem, SimpleVkUser)>> LoadNews()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(NewsItem, SimpleVkUser)>> LoadRecommended()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(Post, SimpleVkUser)>> LoadWall(int pageId)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public void ReceiveNewMessage(LongpollMessage msg)
        {
            OnNewMessage.Invoke(msg);
        }

        public Task<(int?, int?)?> Repost(int ownerId, int postId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendMessage(int peerId, string text)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendWall(int peerId, int ownerId, int postId, string text = null)
        {
            throw new NotImplementedException();
        }

        public void StartLongPoll()
        {
            throw new NotImplementedException();
        }

        public void StopLongPoll()
        {
            throw new NotImplementedException();
        }

        public Task WriteComment(int ownerId, int postId, int replyTo, string text)
        {
            throw new NotImplementedException();
        }
    }
}
