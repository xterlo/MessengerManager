using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerManager.Core;

namespace MessengerManager.MVVM.ViewModel
{
    public class MainWindowViewModel : ObserverObject, INotifyPropertyChanged
    {
        MessangerPageViewModel messangerPageViewModel;

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        #region ProfileSettings

        private string _profileName = "Xterlo";
        public string ProfileName
        {
            get { return _profileName; }
            set { 
                _profileName = value;
                OnPropertyChanged(nameof(ProfileName)); 
            }
        }

        #endregion

        #region ICommands

        public ICommand _OnVkClick { get; set; }

        #endregion

        #region RelayCommands

        //public RelayCommand OnVkClick { get; set; }

        #endregion

        #region implementing functions

        private void VkClick()
        {
            if (messangerPageViewModel is null)
            {
                ISocialNetwork socialNetwork = new VkHandler("", "");
                socialNetwork.Authorization();
                messangerPageViewModel = new MessangerPageViewModel(socialNetwork);
                CurrentView = messangerPageViewModel;
            }
            else
                CurrentView = messangerPageViewModel;
            Console.WriteLine("END");
        }

        #endregion

        public MainWindowViewModel()
        {
            _OnVkClick = new RelayCommand(VkClick);
            CurrentView = new HomeViewModel();

        }
    }
}
