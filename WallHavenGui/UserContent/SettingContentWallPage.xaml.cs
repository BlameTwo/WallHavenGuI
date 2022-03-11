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
    public sealed partial class SettingContentWallPage : Expander
    {

        WallHevenSettingResource home = new WallHevenSettingResource();
        public SettingContentWallPage()
        {
            this.InitializeComponent();
            if (!string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.WallPageSize)))
            {
                string a = home.SettingGetConfig(AppSettingArgs.WallPageSize);
                Radios.SelectedIndex = int.Parse(a);
            }
            else
            {
                Radios.SelectedIndex = 2;
            }
            if (!string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.ProStop)))
                WaitSwitch.IsOn = System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.ProStop));
            else
                WaitSwitch.IsOn = false;
            if (!string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.NewWindow)))
                NewWindow.IsOn = System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.NewWindow));
            else
                NewWindow.IsOn = false;
        }

        private void Radios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var b = sender as RadioButtons;
            int a = b.Items.Count();
            int c = b.SelectedIndex;
            string colorName = (sender as RadioButtons).SelectedItem as string;
            string index = "-1";
            if (sender is RadioButtons rb)
            {
                string stringname = rb.SelectedItem as string;
                switch (stringname)
                {
                    case "高清":
                        index = "0";
                        break;
                    case "超清":
                        index = "1";
                        break;
                    case "原图":
                        index = "2";
                        break;
                }
                home.SettingSetConfig(AppSettingArgs.WallPageSize, index);
            }
            
        }


        private void WaitSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            home.SettingSetConfig(AppSettingArgs.ProStop, WaitSwitch.IsOn.ToString());
        }

        private void NewWindow_Toggled(object sender, RoutedEventArgs e)
        {
            home.SettingSetConfig(AppSettingArgs.NewWindow, NewWindow.IsOn.ToString());
        }
    }
}
