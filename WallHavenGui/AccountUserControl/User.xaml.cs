using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WallEventGUI.Model;
using WallHavenGui.Account;
using WallHavenGui.Account.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.AccountUserControl
{
    public sealed partial class User : UserControl
    {
        public User()
        {
            this.InitializeComponent();
            if (string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.UserLogin)))
                GoOut.Content = "登录";
            else
                GoOut.Content = "退出登录";
        }
        WallHevenSettingResource home = new WallHevenSettingResource();
        ResultUserModel UserModel = new ResultUserModel();
        Account.Model.Account account = new Account.Model.Account();
        WebMoreApi api = new WebMoreApi();
        public async Task Show()
        {
            GoOut.IsEnabled = false;
            Pro.IsIndeterminate = true;
            var vault = new PasswordVault();
            var list = vault.FindAllByResource(AppSettingArgs.AppName);
            var credential = vault.Retrieve(AppSettingArgs.AppName, home.SettingGetConfig(AppSettingArgs.UserLogin));
            var password = credential.Password;
            var result =  await account.Login(home.SettingGetConfig(AppSettingArgs.UserLogin), password);           //进行登录，刷新cookie
            UserModel =  await api.GetUserItem(home.SettingGetConfig(AppSettingArgs.UserLogin));
            UserImage.ImageSource = new BitmapImage(new Uri("https:"+UserModel.UserImage));
            LabelTip.Text = UserModel.UserTip;
            LastLabel.Text = UserModel.LastAction;
            GoOut.IsEnabled = true;
            NameLabel.Text = UserModel.UserName;
            Pro.IsIndeterminate = false;



        }

        void Clear()
        {
            NameLabel.Text = "";
            UserImage.ImageSource = null;
            LabelTip.Text = "";
            LastLabel.Text = "";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(GoOut.Content.ToString() == "退出登录")
            {
                await account.Loginout();
                Clear();
                ContentDialog content = new ContentDialog();
                Login login = new Login();
                login.Dialog = content;
                content.Content = login;
                content.CloseButtonText = "关闭";
                await content.ShowAsync();
            }
            else
            {
                string login2 = home.SettingGetConfig(AppSettingArgs.UserLogin);
                if (login2 == null || string.IsNullOrWhiteSpace(login2))
                {
                    ContentDialog content = new ContentDialog();
                    Login login = new Login();
                    login.Dialog = content;
                    content.Content = login;
                    content.CloseButtonText = "关闭";
                    var a = await content.ShowAsync();
                }
            }
            if (!string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.UserLogin)))
            {
                GoOut.Content = "退出登录";
                await Show();
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await UpdateMyLike(sender as Button);
        }

        private async Task UpdateMyLike(Button button)
        {
            button.IsEnabled = false;
            MyLikes.Items.Clear();
            Pro.IsIndeterminate = true;
            var value = await api.GetMyCommectionsAsync();
            for (int i = 0; i < value.Count; i++)
            {
                MyLikes.Items.Add(value[i]);
            }
            Pro.IsIndeterminate = false;

            button.IsEnabled = true; 
        }
    }
}
