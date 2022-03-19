using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
using WallHavenGui.Account;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.AccountUserControl
{
    public sealed partial class User : UserControl
    {
        public User()
        {
            this.InitializeComponent();
        }
        WallHevenSettingResource home = new WallHevenSettingResource();


        public async void Show()
        {
            var vault = new PasswordVault();
            var list = vault.FindAllByResource(AppSettingArgs.AppName);
            var credential = vault.Retrieve(AppSettingArgs.AppName, home.SettingGetConfig(AppSettingArgs.UserLogin));
            var password = credential.Password;
            Account.Model.Account account = new Account.Model.Account();
            var result =  await account.Login(home.SettingGetConfig(AppSettingArgs.UserLogin), password);           //进行登录，刷新cookie
            WebMoreApi api = new WebMoreApi();
            var a =  await api.GetUserItem(home.SettingGetConfig(AppSettingArgs.UserLogin));
        }
    }
}
