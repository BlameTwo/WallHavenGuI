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
    public sealed partial class Account : Expander
    {
        public Account()
        {
            this.InitializeComponent();
            KeyText.Password = String.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.OpenKey))?"": home.SettingGetConfig(AppSettingArgs.OpenKey);
            
        }

        WallHevenSettingResource home = new WallHevenSettingResource();
        private void MyExpander_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            KeyText.Width = this.ActualWidth - 300;
        }
        int clickcount = 0;
        ContentDialog dialog = new ContentDialog();
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveKey.IsEnabled = false;
            var result = await home.KeyExists(KeyText.Password);
            dialog.PrimaryButtonText = "了解";
            if (result == false)
            {
                dialog.Title = "密钥无效";
                TextBlock textBlock = new TextBlock() { Text = "您可以再次生成一个密钥重新尝试" };
                dialog.Content = textBlock;
                dialog.DefaultButton = ContentDialogButton.Primary;
                await dialog.ShowAsync();
                return;
            }
            else
            {
                home.SettingSetConfig(AppSettingArgs.OpenKey, KeyText.Password);
                home.SettingSetConfig(AppSettingArgs.Is18, false.ToString());
                dialog.Title = "保存成功！";
                TextBlock textBlock = new TextBlock() { Text = "这意味这您的身份从访客升级至用户！" };
                dialog.Content = textBlock;
                dialog.DefaultButton = ContentDialogButton.Primary;
                await dialog.ShowAsync();
                if (clickcount >= 3)
                {
                    clickcount = 0;
                    Open18.Visibility = Visibility.Visible;
                }
                clickcount++;
            }
            SaveKey.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            home.DeleteKey(AppSettingArgs.OpenKey);
        }


        private async void Open18_Click(object sender, RoutedEventArgs e)
        {
            if (!System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.Key18)))
            {
                ContentDialog opendialog = new ContentDialog();
                Open open = new Open();
                
                open.Dialog = opendialog;
                opendialog.Content = open;
                await opendialog.ShowAsync();
            }
            else
            {

            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WallHavenGui.Account.Model.Account account = new WallHavenGui.Account.Model.Account();
            var value =  await account.Login("ZYF8899", "qwe262953");

        }
    }
}
