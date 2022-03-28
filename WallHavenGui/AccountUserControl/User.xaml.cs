using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WallEventGUI.Model;
using WallHavenGui.Account;
using WallHavenGui.Account.AccountPage;
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
            RefLike.IsEnabled = false;
            var vault = new PasswordVault();
            var list = vault.FindAllByResource(AppSettingArgs.AppName);
            var credential = vault.Retrieve(AppSettingArgs.AppName, home.SettingGetConfig(AppSettingArgs.UserLogin));
            var password = credential.Password;
            var result =  await account.Login(home.SettingGetConfig(AppSettingArgs.UserLogin), password);           //进行登录，刷新cookie
            //UserModel =  await api.GetUserItem(home.SettingGetConfig(AppSettingArgs.UserLogin));
            UserModel = result.Results;
            UserImage.ImageSource = new BitmapImage(new Uri("https:"+UserModel.UserImage));
            LabelTip.Text = UserModel.UserTip;
            LastLabel.Text = UserModel.LastAction;
            GoOut.IsEnabled = true;
            NameLabel.Text = UserModel.UserName;
            GoOut.Content = "退出登录";
            Pro.IsIndeterminate = false;
            RefLike.IsEnabled = true;
        }

        void Clear()
        {
            NameLabel.Text = "";
            UserImage.ImageSource = null;
            LabelTip.Text = "";
            LastLabel.Text = "";
            MyFrame.Content = new TextBlock() { Text= "请点击登录按钮查看详细信息",FontSize=30,HorizontalAlignment = HorizontalAlignment.Center ,VerticalAlignment = VerticalAlignment.Center } ;
            Navgite.SelectedItem = null;
            MyLikes.ItemsSource = new ObservableCollection<MyCommections>();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            Pro.IsIndeterminate = true;
            if (GoOut.Content.ToString() == "退出登录")
            {
                await account.Loginout();
                Clear();
                ContentDialog content = new ContentDialog();
                Login login = new Login();
                login.Dialog = content;
                content.Content = login;
                content.CloseButtonText = "关闭";
                await content.ShowAsync();
                if (!string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.UserLogin)))
                    GoOut.Content = "退出登录";
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
                    GoOut.Content = "退出登录";
                }
            }

            if (!string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.UserLogin)))
            {
                GoOut.Content = "退出登录";
                await Show();
            }
            button.IsEnabled = true;
            Pro.IsIndeterminate = false;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await UpdateMyLike(RefLike);
        }

        private async Task UpdateMyLike(Button button)
        {
            button.IsEnabled = false;
            try{MyLikes.Items.Clear();}catch (Exception){}          //try一下吧，因为try是因为对自己代码的不自信，我很少try，这个是因为控件被刷新，items没有更新，懒得去写代码了
            Pro.IsIndeterminate = true;
            var value = await api.GetMyCommectionsAsync();
            MyLikes.ItemsSource = value;
            Pro.IsIndeterminate = false;

            button.IsEnabled = true; 
        }

        private void MyLikes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = MyLikes.SelectedItem as MyCommections;
            MyFrame.Navigate(typeof(WallHavenGui.Account.AccountPage.Like),item);
        }

        private void MyFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (MyFrame.SourcePageType == typeof(WallHavenGui.Account.AccountPage.Like))
                Navgite.SelectedItem = Like;
            if(MyFrame.SourcePageType == typeof(WallHavenGui.Account.AccountPage.Comment))
                Navgite.SelectedItem = Comment;
        }

        private void Navgite_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if(args.SelectedItem == Comment)
            {
                MyFrame.Navigate(typeof(Comment), UserModel);
            }
        }
    }
}
