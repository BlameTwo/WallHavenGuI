using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
using WallHavenGui.AccountUserControl;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WallHavenGui.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DocumentPage : Page
    {
        public DocumentPage()
        {
            this.InitializeComponent();
            Loaded += DocumentPage_Loaded;
        }


        WallHevenSettingResource home = new WallHevenSettingResource();
        private async void DocumentPage_Loaded(object sender, RoutedEventArgs e)
        {
            string login2 = home.SettingGetConfig(AppSettingArgs.UserLogin);
            if (login2 == null ||string.IsNullOrWhiteSpace(login2))
            {
               ContentDialog content = new ContentDialog();
               Login login = new Login();
               login.Dialog = content;
               content.Content = login;
               content.CloseButtonText = "关闭";
               var a = await content.ShowAsync();
            }
            if (!string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.UserLogin)))
                await MyUser.Show();          //刷新控件中的数据
        }
    }
}
