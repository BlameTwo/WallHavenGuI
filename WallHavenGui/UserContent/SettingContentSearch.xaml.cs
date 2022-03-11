using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
using WallHavenGui.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static WallEventGUI.Model.UIModel;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WallHavenGui.UserContent
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingContentSearch : Expander
    {
        public SettingContentSearch()
        {
            this.InitializeComponent();
            FanYiItems.Items.Add(new BDFY() { Name = "中--英", from = "zh", to = "en" });
            FanYiItems.Items.Add(new BDFY() { Name = "中--日", from = "zh", to = "jp" });
            Init();
        }

        private void Init()
        {
            SwitchOpen.IsOn = System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.SearchSave)) ? true : false;
            FanYiItems.SelectedIndex = home.SettingGetConfig(AppSettingArgs.BaiduFanYiTo) == "en" ? FanYiItems.SelectedIndex = 0 : FanYiItems.SelectedIndex = 1;
        }

        WallHevenSettingResource home = new WallHevenSettingResource();
        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (SwitchOpen.IsOn == true)
            {
                home.SettingSetConfig(AppSettingArgs.SearchSave, "True");
            }
            else
            {
                home.SettingSetConfig(AppSettingArgs.SearchSave, "False");
            }
        }

        private void FanYiItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BDFY fy = e.AddedItems[0] as BDFY;  
            if (fy != null)
            {
                home.SettingSetConfig(AppSettingArgs.BaiduFanYiFrom, fy.from);
                home.SettingSetConfig(AppSettingArgs.BaiduFanYiTo, fy.to);
            }
        }
    }
}
