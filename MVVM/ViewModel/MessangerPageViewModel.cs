using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MessengerManager.Core;

namespace MessengerManager.MVVM.ViewModel
{
    public class MessangerPageViewModel : ObserverObject
    {
        private ObservableCollection<MessageLayoutViewModel> _messages;
        public ObservableCollection<MessageLayoutViewModel> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        private string _accountName;
        public string AccountName
        {
            get { return _accountName; }
            set { _accountName = value;
                OnPropertyChanged(nameof(AccountName));
            }
        }

        private string _accountSurname;
        public string AccountSurname
        {
            get { return _accountSurname; }
            set
            {
                _accountSurname = value;
                OnPropertyChanged(nameof(AccountSurname));
            }
        }

        private BitmapImage _accountImage;
        public BitmapImage AccountImage
        {
            get { return _accountImage; }
            set
            {
                _accountImage = value;
                OnPropertyChanged(nameof(AccountImage));
            }
        }

        public MessangerPageViewModel(ISocialNetwork network)
        {
            Messages = new ObservableCollection<MessageLayoutViewModel>();
            AccountName = network.accountName.Split(' ')[0];
            if (network.accountName.Split(' ').Length > 1)
                AccountSurname = network.accountName.Split(' ')[1];
            else
                AccountSurname = "";

            SetImage(network.accountImage);
            List<ProfileMini> dialogsProfiles = network.GetDialogs();
            foreach ( var dialog in dialogsProfiles)
            {
                Messages.Add(dialog);
            }

        }

        private async void SetImage(string url)
        {
            var httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var responseStream = await httpClient.GetStreamAsync(url);
            var bitmapImage = new BitmapImage();

            using (var memoryStream = new MemoryStream())
            {
                await responseStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            AccountImage = bitmapImage;
        }
    }
}
