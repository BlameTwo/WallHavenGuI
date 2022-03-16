    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WallEventGUI.Model;
using WallEventGUI.WallHavenTools;
using WallHavenGui.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.System.UserProfile;
using static WallEventGUI.Model.UIModel;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using WallHavenGui.UserContent;
using System.Net;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WallHavenGui.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WallpaperPage : Page
    {

        string full, file;
        public WallpaperPage()
        {
            this.InitializeComponent();

        }

        Wallpaper MyWallpaper { get; set; }
        string ApiKey { get; set; }
        bool OpenKey { get; set; }

        Downloads dl = new Downloads();
        private bool isDownloading;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(AppBar);
           MyWallpaper = e.Parameter as Wallpaper;
            base.OnNavigatedTo(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;      //顶部Frame
            if (rootFrame.CanGoBack)
                rootFrame.GoBack();
            else
                Window.Current.Close();     //这是处理bug方式，也就是顶部导航没有上一页
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            BackButton.Click-=Button_Click;
        }

        WallFile wallfile = new WallFile();
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            WallHevenSettingResource home = new WallHevenSettingResource();
            ApiKey = home.SettingGetConfig(AppSettingArgs.OpenKey) == "" ? "" : home.SettingGetConfig(AppSettingArgs.OpenKey);
            OpenKey =System.Convert.ToBoolean( home.SettingGetConfig(AppSettingArgs.Is18));
            Tools tools = new Tools() { Key = ApiKey, OpenKey = this.OpenKey };
            MyWallpaper = await tools.GetWallpaperAsync(MyWallpaper.id);
            full = MyWallpaper.ImageType == "image/jpeg" ? ".jpg" : ".png";
            file = MyWallpaper.id + full;
            if (MyWallpaper != null)
            {
                Init(home);
            }
            WallXml xml = new WallXml();
            if(await wallfile.FileExites())
            {
                if (await xml.SmallExits(MyWallpaper.id, "Default"))
                {
                    DefaultName.IsEnabled = false;
                }
            }
        }

        void Init(WallHevenSettingResource home)
        {
            string size = home.SettingGetConfig(AppSettingArgs.WallPageSize);
            Wallpaper.Source =
                size == "0" ? Wallpaper.Source = new BitmapImage(new Uri(MyWallpaper.Thumbs.original)) :
                size == "1" ? Wallpaper.Source = new BitmapImage(new Uri(MyWallpaper.Thumbs.large)) :
                size == "2" ? Wallpaper.Source = new BitmapImage(new Uri(MyWallpaper.WallpaperUrl)) :
                Wallpaper.Source = new BitmapImage(new Uri(MyWallpaper.WallpaperUrl));

            bool ProStop = System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.ProStop));

            if (ProStop)        //等待加载完毕为真
            {
                DownPic.IsEnabled = false;
                DownPath.IsEnabled = false;
                SetWallpaper.IsEnabled = false;
                Wallpaper.ImageOpened += Wallpaper_ImageOpened1;
            }
            else//为假
            {
                Wallpaper.ImageOpened += Wallpaper_ImageOpened;
            }

            Wallpaper.Source = new BitmapImage(new Uri(MyWallpaper.WallpaperUrl));
            ColorGridView.ItemsSource = MyWallpaper.colors;
            TagGridView.ItemsSource = MyWallpaper.Tags;
            Size.Text = MyWallpaper.resolution;
            SourceText.Text = String.IsNullOrWhiteSpace(MyWallpaper.source) ? "无源" : MyWallpaper.source;
            UserImage.ImageSource = new BitmapImage(new Uri(MyWallpaper.User.Images.W128px));
            UserName.Content = MyWallpaper.User.UserName;
            WallpaperViews.Text = MyWallpaper.Views;
            WallpaperSize.Text = MyWallpaper.FileSize + "B" + "--" + MyWallpaper.ImageType;
            WallpaperPurity.Content = MyWallpaper.Purity;
            Link.Content = MyWallpaper.url;
            Farvorites.Text = MyWallpaper.favorites;
        }

        private void Wallpaper_ImageOpened1(object sender, RoutedEventArgs e)
        {

            DownPic.IsEnabled = true;
            DownPath.IsEnabled = true;
            SetWallpaper.IsEnabled = true;
            RingPro.IsActive = false;
        }

        private void Wallpaper_ImageOpened(object sender, RoutedEventArgs e)
        {
            RingPro.IsActive = false;
        }

        private async void Link_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(Link.Content.ToString())) ;
        }



        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            BackButton.Click -= Button_Click;
            this.Loaded -= Page_Loaded;
            Link.Click -= Link_Click;
            TagGridView.SelectionChanged -= TagGridView_SelectionChanged;
            Wallpaper.ImageOpened -= Wallpaper_ImageOpened;
            Wallpaper.ImageOpened -= Wallpaper_ImageOpened1;
            base.OnNavigatingFrom(e);
        }


        void ToSearch(string keyword,string purity)
        {
            WallHevenSettingResource home = new WallHevenSettingResource();
            bool OpenWindow =System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.NewWindow));
            SearchArgs args = new SearchArgs()
            {
                KeyWord = keyword,
            };
            switch (purity)
            {
                case "SFW":
                    args.PurityString = "100";
                    break;
                case "Sketchy":
                    args.PurityString = "010";
                    break;
                case "NSFW":
                    args.PurityString = "001";
                    break;
            }
            if (OpenWindow)
            {
                args.NewWindow = true;
                newWIndow(args);
            }
            else
            {
                args.NewWindow = false;
                oldWindow(args);
            }
        }

        async void newWIndow(SearchArgs args)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();

                frame.Navigate(typeof(SearchPage), args);
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
                
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);

        }

        void oldWindow(SearchArgs args)
        {
            Frame frame = Window.Current.Content as Frame;
            if (args != null)
            {
                frame.Navigate(typeof(SearchPage), args);
            }
        }

        private void TagGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WallpaperTag tag = TagGridView.SelectedItem as WallpaperTag;
            ToSearch("id:" + tag.TagId, WallpaperPurity.Content.ToString());
        }

        


        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var folder = KnownFolders.PicturesLibrary;
            folder =await folder.CreateFolderAsync("WallHavenImages");
            DownloadTip.Title =  await Downloads.SaveImage(MyWallpaper.WallpaperUrl, folder,this.file);
            DownloadTip.Subtitle = DownloadTip.Title == "下载成功！" ? "已经保存到本机图片库中" : "下载失败了哦！请检查网络。";
            DownloadTip.IsOpen = true;
        }

        

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FolderPicker pick = new FolderPicker();
            pick.CommitButtonText = "选择该文件夹";
            var folder = await pick.PickSingleFolderAsync();
            if (folder != null)
            {
                DownloadTip.Title = await Downloads.SaveImage(MyWallpaper.WallpaperUrl, folder,this.file);
                DownloadTip.Subtitle = DownloadTip.Title == "下载成功！" ? "已经保存到目标文件夹" : "下载失败了哦！请检查网络。";
                DownloadTip.IsOpen = true;
            }
        }

        [Obsolete]
        private async void WallpaperScroll_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var doubleTapPoint = e.GetPosition(scrollViewer);

            if (scrollViewer.ZoomFactor != 1)
            {
                //双击把缩放定位1
                scrollViewer.ZoomToFactor(1);
            }
            else if (scrollViewer.ZoomFactor == 1)
            {
                scrollViewer.ZoomToFactor(2);
                var dispatcher = Window.Current.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    scrollViewer.ScrollToHorizontalOffset(doubleTapPoint.X);
                    scrollViewer.ScrollToVerticalOffset(doubleTapPoint.Y);
                });
            }
        }


        private void UserName_Click(object sender, RoutedEventArgs e)
        {
            ToSearch("@" + MyWallpaper.User.UserName, WallpaperPurity.Content.ToString());
        }

        private void WallpaperPurity_Click(object sender, RoutedEventArgs e)
        {
            ToSearch("", WallpaperPurity.Content.ToString());
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ToSearch("like:" + MyWallpaper.id, WallpaperPurity.Content.ToString());
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            WallXml xml = new WallXml();
            WallFile wallfile = new WallFile();
            List<Wallpaper> wallpaperlist = new List<Wallpaper>();  
            wallpaperlist.Add(MyWallpaper);
            xml.SaveDefaultImageId(await wallfile.FileExites(), wallpaperlist, "Default");
            DefaultName.IsEnabled = false;
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "选择你对应的收藏夹";
            var content = new SmallsContent();
            content.MyData = new List<Wallpaper>() { MyWallpaper };
            content.FatherData = "Default";
            content.Dialog = dialog;    //传入dialog
            dialog.Content = content;
            await dialog.ShowAsync();
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (UserProfilePersonalizationSettings.IsSupported())
            {
                StorageFolder folider = Windows.Storage.ApplicationData.Current.LocalFolder;
                try
                {
                    folider = await folider.GetFolderAsync("DownloadImage");
                }
                catch (Exception)
                {
                    folider = await folider.CreateFolderAsync("DownloadImage");//创建文件夹
                }
                string stringresult = await Downloads.SaveImage(MyWallpaper.WallpaperUrl, folider, file);
                if (!(stringresult == "下载成功"))
                {
                    UserProfilePersonalizationSettings setting = UserProfilePersonalizationSettings.Current;
                    StorageFile file2 = await folider.GetFileAsync(file);
                    bool result2 = await setting.TrySetWallpaperImageAsync(file2);
                    DownloadTip.Title = "提示";
                    DownloadTip.Subtitle = result2 ? "设置成功！" : "设置失败！请先保存图片！";
                    DownloadTip.IsOpen = true;
                }
            }
            else
            {
                DownloadTip.Title = "提示";
                DownloadTip.Subtitle = "当前机器不支持设置桌面哦";
                DownloadTip.IsOpen = true;
            }
        }
    }
}
