using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MessengerManager
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            RegistryKey currentUserSoftware = Registry.CurrentUser.OpenSubKey("Software",true);
            if (currentUserSoftware.OpenSubKey("MessengerManager") == null)
            {
                currentUserSoftware.CreateSubKey("MessengerManager", true);
            };
            RegistryKey messengerManager = currentUserSoftware.OpenSubKey("MessengerManager", true);
            bool vkEsist = false;
            bool tgEsist = false;
            foreach(string key in messengerManager.GetValueNames())
            {
                if (key == "VK") vkEsist = true;
                if (key == "TG") tgEsist = true;
            }
            if (!vkEsist) messengerManager.SetValue("VK", "");
            if (!tgEsist) messengerManager.SetValue("TG", "");
            messengerManager.Close();
            base.OnStartup(e);
        }
    }
}
