using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
    public sealed partial class SettingContentLike : Expander
    {
        public SettingContentLike()
        {
            this.InitializeComponent();
            if (!string.IsNullOrWhiteSpace(setting.SettingGetConfig(AppSettingArgs.LikePage)))          //有设置的情况下
            {
                    NumberBox.Value = Convert.ToDouble(setting.SettingGetConfig(AppSettingArgs.LikePage));
            }
            else
            {
                NumberBox.Value = 24;
            }
        }

        WallHevenSettingResource setting = new WallHevenSettingResource();

       

        private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if(args.NewValue != args.OldValue)
                setting.SettingSetConfig(AppSettingArgs.LikePage, args.NewValue.ToString());
        }
    }
}
