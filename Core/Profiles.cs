using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using System.Linq;
using VkNet.Enums.SafetyEnums;
using MessengerManager.MVVM.ViewModel;

namespace MessengerManager.Core
{
    public class ProfileMini : MessageLayoutViewModel
    {

       public string ProfileName { get; set; }
       public string ProfileLastName { get; set; }
       public string ProfileImage { get; set; }
       public long ProfileId { get; set; }
       public string lastMessage { get; set; }
       public bool isMyMessage { get; set; }
       public bool isChat { get; set; }
       public bool isGroupChat { get; set; }
       public string chatImage { get; set; }
       public string chatTitle { get; set; }



        public ProfileMini(ConversationAndLastMessage dialog, User userInfo)
        {
            var lastMessageDialog = dialog.LastMessage;
            var chatSettings = dialog.Conversation.ChatSettings;

            chatTitle = $"{userInfo.FirstName} {userInfo.LastName}";
            chatImage = userInfo.Photo200 is null ? "https://vk.com/images/camera_200.png" : userInfo.Photo200.AbsoluteUri;
            ProfileName = userInfo.FirstName;
            ProfileLastName = userInfo.LastName;
            ProfileImage = userInfo.Photo200.AbsoluteUri;
            lastMessage = lastMessageDialog.Text;
            isMyMessage = Convert.ToBoolean(lastMessageDialog.Out);
            ProfileId = (long)lastMessageDialog.FromId;
        }
        public ProfileMini(ConversationAndLastMessage dialog, Group group)
        {
            var lastMessageDialog = dialog.LastMessage;
            var chatSettings = dialog.Conversation.ChatSettings;
            isGroupChat = true;
            lastMessage = lastMessageDialog.Text;
            chatImage = group.Photo200 is null ? "https://vk.com/images/camera_200.png" : group.Photo200.AbsoluteUri;
            chatTitle = group.Name;
        }
        public ProfileMini(ConversationAndLastMessage dialog, Chat chat)
        {

            var lastMessageDialog = dialog.LastMessage;
            isChat = true;
            isGroupChat = true;
            lastMessage = lastMessageDialog.Text;
            chatImage = chat.Photo200 is null ? "https://vk.com/images/camera_200.png" : chat.Photo200;
            
            chatTitle = chat.Title;
        }

    }
}
