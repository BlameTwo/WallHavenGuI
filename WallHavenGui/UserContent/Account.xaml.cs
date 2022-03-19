using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.UserContent
{
    public sealed partial class Account : UserControl
    {
        public Account()
        {
            this.InitializeComponent();
            
        }

        WallHevenSettingResource home = new WallHevenSettingResource();
        int clickcount = 0;
        ContentDialog dialog = new ContentDialog();
        

        

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WallHavenGui.Account.Model.Account account = new WallHavenGui.Account.Model.Account();
            var value =  await account.Login("ZYF8899", "qwe262");
            if(value == null)
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "提示";
                dialog.Content = "账号或密码错误，请重新确认";
                dialog.PrimaryButtonText = "是";
                await dialog.ShowAsync();
                return;
            }
            await account.Loginout();
        }
    }
}
