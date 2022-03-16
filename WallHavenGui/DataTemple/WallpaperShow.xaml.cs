using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.WallHavenTools;
using WallHavenGui.DataTemple.ViewModels;
using WallHavenGui.Model;
using WallHavenGui.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.DataTemple
{
    public sealed partial class WallpaperShow : UserControl
    {
        public WallpaperShow()
        {
            this.InitializeComponent();
            this.Loaded += WallpaperShow_Loaded;
            LikeCommand = new List<RelayCommand<string>>();
        }

        private async void WallpaperShow_Loaded(object sender, RoutedEventArgs e)
        {
            WallFile wallfile = new WallFile();
            if (await wallfile.FileExites() && LikeCommand.Count== 0)           //防止重复添加
            {
                WallXml xml = new WallXml();        //没有本地收藏会报错
                items = await xml.GetBigImage();
                List<MenuFlyoutItem> list = new List<MenuFlyoutItem>();
                if(items == null)
                    return;
                foreach (var i in items)
                {
                    if (i.Name == "Default")
                        continue;
                    MenuFlyoutItem item = new MenuFlyoutItem();
                    item.Text = i.Name;
                    RelayCommand<string> relays;
                    relays = new RelayCommand<string>(se =>
                    {
                        xml.AddImageedId(new List<Wallpaper>() { MyData }, i.Name);
                    });
                    LikeCommand.Add(relays);            //把命令加入到集合中
                    item.Command = relays;          //设置命令
                    item.CommandParameter = i.Name;             //设置附带参数
                    list.Add(item);
                    LikeMenu.Items.Add(item);
                }
            }
        }

        List<RelayCommand<string> >LikeCommand { get; set; }

        ObservableCollection<BigClass> items = new ObservableCollection<BigClass>();
        

        public Wallpaper MyData
        {
            get { return (Wallpaper)GetValue(MyDataProperty); }
            set { SetValue(MyDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyDataProperty =
            DependencyProperty.Register(
                "MyData", 
                typeof(Wallpaper), 
                typeof(WallpaperShow), 
                new PropertyMetadata(null));


        Downloads dl = new Downloads();

        private async void SavePath_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker pick = new FolderPicker();
            pick.CommitButtonText = "选择该文件夹";
            var folder = await pick.PickSingleFolderAsync();
            if (folder != null)
            {
                string nu = MyData.ImageType == "image/jpeg" ? ".jpg" : ".png";
                string type =$"{MyData.id}{nu}";
                DownloadTask.AddAsync(Downloads.SaveImage(MyData.WallpaperUrl, folder, type));
            }
        }

        private  void SavePictureLibary_Click(object sender, RoutedEventArgs e)
        {
            var folder = KnownFolders.PicturesLibrary;
            string nu = MyData.ImageType == "image/jpeg" ? ".jpg" : ".png";
            string type = MyData.id + nu;
            DownloadTask.AddAsync(Downloads.SaveImage(MyData.WallpaperUrl, folder, type));
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Frame rootframe = Window.Current.Content as Frame;
            rootframe.Navigate(typeof(WallpaperPage), MyData, new DrillInNavigationTransitionInfo());
        }

        private void DefaultLike_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
