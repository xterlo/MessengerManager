using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MessengerManager.Core
{
    public enum SocialNetwork
    {
        VK,
        TG
    }

    public interface ISocialNetwork
    {
        //void SendMessage();
        void GetProfileInfo();
        List<ProfileMini> GetDialogs();
        void Authorization();
        SocialNetwork socialNetwork { get;}
        string accountName { get;}
        string accountLogin { get; }
        string accountPassword { get; }
        string accountToken { get; }
        string accountImage { get; }
        long accountId { get; }
    }

    public class ResponseHandler
    {
        private static readonly HttpClient client = new HttpClient();
        private string url;
        ISocialNetwork network;
        Dictionary<string, string> parameters;

        public ResponseHandler(ISocialNetwork network)
        {
            this.network = network;
            switch (network.socialNetwork)
            {
                case SocialNetwork.VK:
                    url = "https://api.vk.com/oauth/token";
                     parameters = new Dictionary<string, string>
                    {
                        {"grant_type","password" },
                        {"client_id","2274003" },
                        {"scope","offline" },
                        {"client_secret","hHbZxrka2uZ6jB1inYsH" },
                        {"username",network.accountLogin },
                        {"password",network.accountPassword }
                    };
                    break;
            }

        }

        


    }
}
