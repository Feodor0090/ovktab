using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.OvkTab.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace osu.Game.Rulesets.OvkTab.Tests.Stubs
{
    [Cached]
    internal class OvkApiHubStub : IOvkApiHub
    {
        public BindableBool IsLongpollFailing => throw new NotImplementedException();

        public Bindable<SimpleVkUser> LoggedUser { get => loggedUser; }

        private readonly Bindable<SimpleVkUser> loggedUser = new();
        public string Token => "Maho-sempai";

        public int UserId => 727;

        public event Action<LongpollMessage> OnNewMessage;
        public event Action<LongpollMessageEdit> OnMessageEdit;

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
            Comment first = new()
            {
                FromId = 292,
                Text = "vk4me is shit",
                Thread = new CommentThread
                {
                    Count = 1,
                    Items = new System.Collections.ObjectModel.ReadOnlyCollection<Comment>(new Comment[]
                    {
                        new()
                        {
                            FromId = 292,
                            Text = "vkm is better",
                            Likes = new Likes { Count = 131, CanLike = true },
                        }
                    })
                },
                Likes = new Likes { Count = 4, CanLike = true },
            };
            Comment second = new()
            {
                FromId = 292,
                Text = "donate pls",
                Likes = new Likes { Count = 2011, CanLike = true },
            };
            return Task.FromResult<(IEnumerable<Comment>, SimpleVkUser[], int, bool, bool)>((new[] {first, second}, new[] { Maho }, 3, true, true));
        }

        public Task<IEnumerable<(SimpleVkUser, ConversationAndLastMessage)>> GetDialogsList()
        {
            List<(SimpleVkUser, ConversationAndLastMessage)> a = new(new (SimpleVkUser, ConversationAndLastMessage)[]
            {
                (null, new ConversationAndLastMessage
                {
                    Conversation = new Conversation
                    {
                        ChatSettings = new ConversationChatSettings
                        {
                            Photo = new Photo
                            {
                                Photo100 = new Uri("https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/81/816774a678a42aa1100875e4779ad5cb9a017d17_full.jpg"),
                            },
                            Title = "nnproject.cc shitposting",
                        },
                        Peer = new Peer { Id = 1296000 }
                    }
                }), (Maho, new ConversationAndLastMessage
                {
                    Conversation = new Conversation
                    {
                        Peer = new Peer { Id = 1296000 }
                    }
                }),
            });
            for(int i=0; i<50; i++)
            {
                a.Add((null, new ConversationAndLastMessage
                {
                    Conversation = new Conversation
                    {
                        ChatSettings = new ConversationChatSettings
                        {
                            Photo = new Photo
                            {
                                Photo100 = new Uri("https://yt3.ggpht.com/a/AATXAJwoQewWpap2Hpsdzn3Ai3aIdCdTQPXDYgJPEALz=s900-c-k-c0x00ffffff-no-rj"),
                            },
                            Title = $"Chat eblanov {i+1}/50",
                        },
                        Peer = new Peer { Id = 1296000 }
                    }
                }));
            }
            return Task.FromResult<IEnumerable<(SimpleVkUser, ConversationAndLastMessage)>>(a);
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

        public Task<((SimpleVkUser, Message)[], SimpleVkUser[])> LoadHistory(long peer)
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
            Post p = new()
            {
                OwnerId = 292,
                Id = 1296000,
                Text = @"Обновление моего клиeнтa YouTube для Symbian - JTube!
Основан на Invidious API + собственных наработках.
Главный плюс - это эмулируемое приложение в формате jar,
                написанное на MIDP(без костыльной qt) и тесная интеграция с системным видеоплеером.

Функционал:
-Просмотр видео онлайн в системном проигрывателе без сторонних сайтов и сервисов(на Symbian Anna и выше). На Symbian 9.2 - 9.4 играет тоже, но (временно) через костыль, упомянутый ниже;
            -Поиск видео;
            -Качество видео выбирается автоматически(вплоть до 720р)

Обновление добавило частичную предварительную поддержку Symbian 9.2 - 9.4, управление кнопками(включая прокрутку и навигацию по меню) и оптимизацию под дисплеи с малым разрешением и позволяет скачивать видео и смотреть их онлайн, но через сайт - посредник, next.2yxa.mobi.

   Тестировался на Nokia E51, Nokia E63, Nokia E71, Nokia E72, Nokia N8, Nokia 603, Nokia 808 PureView.

На момент создания поста идёт работа над интеграцией прямого обращения к системному видеоплееру без использования вышеупомянутого костыля, добавление встроенного загрузчика видео(без использования браузера) и добавление кинетической прокрутки вместо линейной на сенсорных устройствах.

Для работы требуется Symbian 9.1 и выше и установленные библиотеки osu!framework версии 2022.118.0 и выше.
Загрузить можно с http://nnproject.cc",
            };

            return Task.FromResult<IEnumerable<(Post, SimpleVkUser)>>(new[] { (p, Maho) });
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
            return Task.FromResult<(int?, int?)?>((1296000, 131));
        }

        public Task<bool> SendMessage(int peerId, string text, int replyTo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendWall(int peerId, int ownerId, int postId, string text = null)
        {
            return Task.FromResult(true);
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
            return Task.CompletedTask;
        }

        public Task<bool> DeleteMessage(int peerId, ulong convMsgId, ulong id)
        {
            throw new NotImplementedException();
        }

        public void ReportRead(int peerId, int msgId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditMessage(int peerId, long convMsgId, int msgId, string newText, bool keepReply, bool keepAtts)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendLink(int peerId, string title, string url, string text, int replyTo)
        {
            throw new NotImplementedException();
        }

        public static SimpleVkUser Maho => new SimpleVkUser
        {
            avatarUrl = "https://sun9-46.userapi.com/s/v1/ig2/CrdTs9uQS4a3hJVoKOK4Dd8jio613BHW-svlRZzj_qrw2mRGPPoJeYQ1ueseW8m7e2NtIfN1afB4jaX_U0m0jkfE.jpg?size=200x241&quality=96&crop=0,0,700,845&ava=1",
            id = 292,
            name = "Angelochek Ebanutyi"
        };
    }
}
