using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using static WallEventGUI.Model.UIModel;
using WallEventGUI.WallHavenTools;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using WallEventGUI.Model;
using WallEventGUI.ConvertModult;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WallHavenGui.UserContent
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingContentHome : Expander
    {
        public SettingContentHome()
        {
            this.InitializeComponent();
            HomeComboBox.Items.Add(new HomeSorting() { Name = "当前最热", Sorting = sorting.toplist });
            HomeComboBox.Items.Add(new HomeSorting() { Name = "随机", Sorting = sorting.random });
            HomeComboBox.Items.Add(new HomeSorting() { Name = "观看人数", Sorting = sorting.views });

            Init();
        }

        private void Init()
        {
            switch (home.SettingGetConfig(AppSettingArgs.HomeSorting))
            {
                case "toplist": HomeComboBox.SelectedIndex = 0; break;
                case "random": HomeComboBox.SelectedIndex = 1; break;
                case "views": HomeComboBox.SelectedIndex = 2; break;
                default: HomeComboBox.SelectedIndex = 0; break;
            }
            switch (home.SettingGetConfig(AppSettingArgs.HomeOrderBy))
            {
                case "desc": DescOrder.IsChecked = true; break;
                case "asc": AscOrder.IsChecked = true; break;
                default: DescOrder.IsChecked = true; break;
            }
            var pritys = UIConvert.BackLevel(home.SettingGetConfig(AppSettingArgs.HomePurityString));
            PurLevel1.IsChecked = pritys.Level1;
            PurLevel2.IsChecked = pritys.Level2;
            PurLevel3.IsChecked = pritys.Level3;

            var pritys2 = UIConvert.BackLevel(home.SettingGetConfig(AppSettingArgs.HomeCategoryString));
            CatLevel1.IsChecked = pritys2.Level1;
            CatLevel2.IsChecked = pritys2.Level2;
            CatLevel3.IsChecked = pritys2.Level3;
        }

        WallHevenSettingResource home = new WallHevenSettingResource();
        

        private void HomeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HomeSorting sorting = e.AddedItems[0] as HomeSorting;
            home.SettingSetConfig(AppSettingArgs.HomeSorting, sorting.Sorting.ToString());
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;  
            if (radioButton.Content.ToString() == "倒序" &&radioButton.IsChecked == true)
            {
                home.SettingSetConfig(AppSettingArgs.HomeOrderBy, "desc");
            }else if(radioButton.Content.ToString() == "正序" && radioButton.IsChecked == true)
            {
                home.SettingSetConfig(AppSettingArgs.HomeOrderBy, "asc");
            }
        }

        private void CatLevel_Checked(object sender, RoutedEventArgs e)
        {
            bool a, b, c;
            a = (bool)CatLevel1.IsChecked;
            b = (bool)CatLevel2.IsChecked;
            c = (bool)CatLevel3.IsChecked;
            LevelCs cs = new LevelCs() { Level1 = a, Level2 = b, Level3 = c };
            string result =  UIConvert.GetLevel(cs);
            home.SettingSetConfig(AppSettingArgs.HomeCategoryString,result);
        }

        private void PurLevel_Checked(object sender, RoutedEventArgs e)
        {
            bool a, b, c;
            a = (bool)PurLevel1.IsChecked;
            b = (bool)PurLevel2.IsChecked;
            c = (bool)PurLevel3.IsChecked;
            LevelCs cs = new LevelCs() { Level1 = a, Level2 = b, Level3 = c };
            string result = UIConvert.GetLevel(cs);
            home.SettingSetConfig(AppSettingArgs.HomePurityString, result);
        }
    }
}
