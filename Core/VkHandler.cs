using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace MessengerManager.Core
{
    public class VkHandler : ISocialNetwork
    {
        private VkApi api;
        private static readonly HttpClient client = new HttpClient();

        public string authUrl = "https://api.vk.com/oauth/token";
        public string apiUrl = "https://api.vk.com/method/";

        public string getProfileInfo = "account.getProfileInfo?";

        private string _accountName;
        private string _accountToken;
        private string _accountImage;
        private string _accountLogin;
        private string _accountPassword;
        private long _accountId;

        public string accountName { get => _accountName; }
        public string accountToken { get => _accountToken; }
        public string accountImage { get => _accountImage; }
        public string accountLogin { get => _accountLogin; }
        public string accountPassword { get => _accountPassword; }
        public long accountId{ get => _accountId; }

        public SocialNetwork socialNetwork => SocialNetwork.VK;

        public VkHandler(params string[] data)
        {
            RegistryKey currentUserSoftware = Registry.CurrentUser.OpenSubKey("Software\\MessengerManager");
            _accountToken = currentUserSoftware.GetValue("VK").ToString();
            _accountLogin = data[0];
            _accountPassword = data[1];
        }

        public void Authorization()
        {

            if (accountToken.Length > 0)
            {
                api = new VkApi();
                api.Authorize(new ApiAuthParams
                {
                    AccessToken = accountToken,
                    
                });
            }
            else
            {
                var services = new ServiceCollection();
                services.AddAudioBypass();
                var api = new VkApi(services);
                api.Authorize(new ApiAuthParams()
                {
                    ApplicationId = 8204123,
                    Login = accountLogin,
                    Password = accountPassword,
                    Settings = Settings.All,
                    TwoFactorSupported = true,
                    TwoFactorAuthorization = () =>
                    {
                        Console.Write("Please enter code: ");
                        string value = Console.ReadLine();

                        return value;
                    }

                });
                RegistryKey currentUserSoftware = Registry.CurrentUser.OpenSubKey("Software\\MessengerManager",true);
                currentUserSoftware.SetValue("VK", api.Token);
            }
            GetProfileInfo();
        }

        public void GetProfileInfo()
        {

            var userInfo = api.Users.Get(new List<long> { }, ProfileFields.All).FirstOrDefault();
            _accountId = userInfo.Id;
            api.UserId = accountId;
            _accountName = $"{userInfo.FirstName} {userInfo.LastName}";
            _accountImage = userInfo.Photo200.AbsoluteUri;
        }

        public List<ProfileMini> GetDialogs()
        {
            List<ProfileMini> profiles = new List<ProfileMini>();
            var dialogs = api.Messages.GetConversations(new GetConversationsParams
            {
                Count = 10
            });

            IEnumerable<long> usersIds = (from profile in dialogs.Items
                                          where profile.Conversation.Peer.Type == ConversationPeerType.User
                                          select profile.Conversation.Peer.Id).ToArray();

            IEnumerable<long> chatsIds = (from chats in dialogs.Items
                                            where chats.Conversation.Peer.Type == ConversationPeerType.Chat
                                            select chats.Conversation.Peer.LocalId).ToArray();

            IEnumerable<string> groupsIds = (from groups in dialogs.Items
                                             where groups.Conversation.Peer.Type == ConversationPeerType.Group
                                             select Math.Abs(groups.Conversation.Peer.Id).ToString()).ToArray();


            var chatsInfo = api.Messages.GetChat(chatsIds);
            var profileInfo = api.Users.Get(usersIds, ProfileFields.All);
            var groupsInfo = api.Groups.GetById(groupsIds,null,GroupsFields.All);

            foreach (var dialog in dialogs.Items)
            {
                if(dialog.Conversation.Peer.Type.Equals(ConversationPeerType.User))
                {
                    var lastUser = (from profile in profileInfo
                                    where profile.Id == dialog.Conversation.Peer.Id
                                    select profile).First();

                    profiles.Add(new ProfileMini(dialog, lastUser));
                }
                else if (dialog.Conversation.Peer.Type.Equals(ConversationPeerType.Group))
                {
                    var lastGroup = (from groups in groupsInfo
                                    where groups.Id == Math.Abs(dialog.Conversation.Peer.Id)
                                    select groups).First();
                    profiles.Add(new ProfileMini(dialog, lastGroup));
                }
                else if (dialog.Conversation.Peer.Type.Equals(ConversationPeerType.Chat))
                {
                    var lastChat = (from chat in chatsInfo
                                     where chat.Id == dialog.Conversation.Peer.LocalId
                                     select chat).First();
                    profiles.Add(new ProfileMini(dialog, lastChat));
                }

            }
            return profiles;
            //= new ProfileMini(dialogs, profileInfo);
        }
    }
}
