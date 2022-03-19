using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
using WallHavenGui.Account.Model;
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
    public sealed partial class Login : UserControl
    {
        public Login()
        {
            this.InitializeComponent();
        }



        int clickcount = 0;

        public ContentDialog Dialog
        {
            get { return (ContentDialog)GetValue(DialogProperty); }
            set { SetValue(DialogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dialog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogProperty =
            DependencyProperty.Register("Dialog", typeof(ContentDialog), typeof(Login), new PropertyMetadata(0));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dialog.Hide();
        }

        WallHevenSettingResource home = new WallHevenSettingResource();
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Pro.IsIndeterminate = true;
            Account.Model.Account account = new Account.Model.Account();
            var result =  await account.Login(username.Text,password.Password);
            if(result == null)
            {
                TipText.Text = "账号或密码错误";
                Pro.IsIndeterminate = false;
                return;
            }
            TipText.Text = "登录成功";
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(AppSettingArgs.AppName, username.Text, password.Password));
            home.SettingSetConfig(AppSettingArgs.OpenKey, result.UserKey);          //保存密钥
            home.SettingSetConfig(AppSettingArgs.UserLogin, username.Text);             //保存账号
            Pro.IsIndeterminate = false;
            
            Dialog.Hide();
        }

        
    }
}
