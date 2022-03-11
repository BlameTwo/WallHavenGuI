using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
using WallEventGUI.WallHavenTools;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.UserContent.LikeWallpaper
{
    public sealed partial class BigImages : UserControl
    {


        bool OpenKey { get; set; }
        string MenuText { get; set; }
        string ApiKey{get; set;}
        WallHevenSettingResource home = new WallHevenSettingResource();
        ObservableCollection<Wallpaper> ContentSource = new ObservableCollection<Wallpaper>();
        int maxint { get; set; }
        ScrollViewer SV = new ScrollViewer();
        public BigImages()
        {
            this.InitializeComponent();
            this.Loaded += BigImages_Loaded;
        }
        int maxpage = 0, nowpage = 0, oldpage = 0;
        WallXml wallxml = new WallXml();
        WallFile file = new WallFile();
        private async void BigImages_Loaded(object sender, RoutedEventArgs e)
        {
            string txt = home.SettingGetConfig(AppSettingArgs.LikePage);
            maxpage = !string.IsNullOrWhiteSpace(txt) ? int.Parse(txt) : 24;
            oldpage = maxpage;
            ApiKey = home.SettingGetConfig(AppSettingArgs.OpenKey) == "" ? "" : home.SettingGetConfig(AppSettingArgs.OpenKey);
            OpenKey = string.IsNullOrWhiteSpace(ApiKey) ? false : true;
            if (await file.FileExites() == true)
            {
                MenuList =  await wallxml.GetBigImage();
                Navtation.MenuItemsSource = MenuList;
            }
        }
        ObservableCollection<BigClass> MenuList = new ObservableCollection<BigClass>();
        [DisplayName]
        private async void Navtation_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            ContentSource.Clear();
            MyContent.ItemsSource = ContentSource;
            BigClass arg =  args.SelectedItem as BigClass;

            if (MenuText != arg.Name)
            {
                nowpage = 0;maxpage = oldpage;
            }
            Tools tools = new Tools(); 
            tools.OpenKey = OpenKey;
            tools.Key = ApiKey;
            if (arg != null)
            {
                var list = await wallxml.GetSmallImage(arg.Name,nowpage, maxpage);
                ContentSource = list;
                MenuText = arg.Name;
                foreach (var item in MyContent.Items)
                {
                    ContentSource.Add(item as Wallpaper);
                }
                MyContent.ItemsSource = ContentSource;
                nowpage += list.Count;
                maxpage += nowpage ;
            }
        }

        private void Navtation_Loaded(object sender, RoutedEventArgs e)
        {
            var listview = (VisualTreeHelper.GetChild(MyContent, 0) as Border).Child as ScrollViewer;
            SV = listview;
            SV.ViewChanged += SV_ViewChanged;
        }

        private void SV_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            var flage = sv.VerticalOffset + sv.ViewportHeight;          //已经滚动的长度
            if (sv.ExtentHeight - flage < 5 && sv.ViewportHeight != 0)
            {
                MoreData();
            }
        }

        private void MoreData()
        {
            SV.ViewChanged-= SV_ViewChanged;
            SV.ViewChanged += SV_ViewChanged;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Selectmode.IsChecked.Value)
            {
                MyContent.SelectionMode = ListViewSelectionMode.Multiple;

            }
            else
            {
                MyContent.SelectionMode = ListViewSelectionMode.Single;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(selectWall.Count!= 0)
            {
                var result = await wallxml.DeleteImage(MenuText, selectWall);
                if (result == true)
                    MyContent.ItemsSource = await wallxml.GetSmallImage(MenuText, nowpage, maxpage);
            }
            
        }

        private void MyContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectWall.Clear();
            if (MyContent.SelectedIndex != -1)
            {
                if (MyContent.SelectionMode == ListViewSelectionMode.Multiple)       //多选
                {
                    foreach (var item in MyContent.SelectedItems)
                    {
                        var result = item as Wallpaper;
                        selectWall.Add(result);
                    }
                }
                else if (MyContent.SelectionMode == ListViewSelectionMode.Single)        //单选
                {
                    var result = MyContent.Items[MyContent.SelectedIndex];
                    selectWall.Add(result as Wallpaper);
                }
            }
        }

        List<Wallpaper> selectWall = new List<Wallpaper>();

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var result = await wallxml.DeleteSmall(MenuText);
            if (result == true)
            {
                MenuList = await wallxml.GetBigImage();
                Navtation.MenuItemsSource = MenuList;
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "选择你对应的收藏夹";
            var content = new SmallsContent();
            content.MyData = selectWall;
            content.FatherData = (Navtation.SelectedItem as BigClass).Name;
            content.Dialog = dialog;    //传入dialog
            dialog.Content = content;
            await dialog.ShowAsync();
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MenuText))
                return;
            var list = await wallxml.GetSmallImage(MenuText,nowpage,maxpage);
            if(list.Count!= 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ContentSource.Add(list[i]);
                }
                MyContent.ItemsSource = ContentSource;
                nowpage += list.Count;
                maxpage += oldpage;
            }
            else
            {
                Tip.Title = "提示";
                Tip.Subtitle = "该文件夹没有返回更多的图片。";
                Tip.IsOpen = true;

            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "请输入文字创建收藏夹";
            var content = new AddBigImage();
            content.Dialog = dialog;
            dialog.Content = content;
            var result = await dialog.ShowAsync();
        }
    }
}
