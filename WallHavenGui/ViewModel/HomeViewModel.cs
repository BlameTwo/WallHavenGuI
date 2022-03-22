using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.Model;
using WallEventGUI.WallHavenTools;
using WallHavenGui.Pages;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace WallEventGUI.ViewModel
{
    public class HomeViewModel: ObservableObject
    {
        public HomeViewModel()
        {
            ScrlLoad = new RelayCommand<GridView>((se)=>scrool(se));
            Walls = new Wallpapers();
        }
        ScrollViewer SV = new ScrollViewer();

        private void scrool(GridView gv)
        {
            var listview = (VisualTreeHelper.GetChild(gv, 0) as Border).Child as ScrollViewer;
            SV = listview;
            SV.ViewChanged += SV_ViewChanged;
        }

        private void SV_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv  = sender as ScrollViewer;
            var flage = sv.VerticalOffset + sv.ViewportHeight;          //已经滚动的长度
            
            if (sv.ExtentHeight - flage<5 && sv.ViewportHeight!=0)
            {
                LinkClick();
            }
            
        }

        sorting Sorting;
        order Order;
        string ApiKey;
        string PruityString;
        string CategoryString;
        bool OpenKey;
        int pagesize = 1;
        int maxpagesize = 0;
        WallHavenTools.Tools tools = new WallHavenTools.Tools();

        public async void load(object sender, RoutedEventArgs e)
        {
            WallHevenSettingResource home = new WallHevenSettingResource();
            switch (home.SettingGetConfig(AppSettingArgs.HomeSorting))
            {
                case "toplist": Sorting = sorting.toplist; break;
                case "random": Sorting = sorting.random; break;
                case "views": Sorting = sorting.views; break;
                default: Sorting = sorting.toplist; break;
            }
            switch (home.SettingGetConfig(AppSettingArgs.HomeOrderBy))
            {
                case "desc":
                    Order = order.desc; break;
                case "asc":
                    Order = order.asc; break;
                default:
                    Order = order.desc;
                    break;
            }
            ApiKey = home.SettingGetConfig(AppSettingArgs.OpenKey)==""?"":home.SettingGetConfig(AppSettingArgs.OpenKey);
            //OpenKey =string.IsNullOrWhiteSpace(ApiKey)?false:true;
            OpenKey = System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.Is18));
            tools.OpenKey = this.OpenKey;
            tools.Key = this.ApiKey;
            PruityString = home.SettingGetConfig(AppSettingArgs.HomePurityString) == null ? "100": home.SettingGetConfig(AppSettingArgs.HomePurityString);
            CategoryString = home.SettingGetConfig(AppSettingArgs.HomeCategoryString) == null ? "100" : home.SettingGetConfig(AppSettingArgs.HomeCategoryString); ;
            _ProCess = true;
            var result = await tools.GetSearchWallpaperString(Sorting, topRange.D1, "", "", CategoryString, PruityString, Order, pagesize, colors.None);
            maxpagesize = int.Parse(result.Meta.Last_Page);
            if (Walls.WallpaperList==null)    //这个意思是有旧数据的话保留，是为了保存上一次查看记录
            {
                Walls = result;
            }
            PageTitle = $"已经加载{pagesize}页,最后一页为{maxpagesize}";
            _ProCess = false;
        }

        private Wallpapers walls;

        public Wallpapers Walls
        {
            get { return walls; }
            set { walls = value; 
                OnPropertyChanged(); }
        }


        public class LevelCs
        {
            public bool Level1 { get; set; }
            public bool Level2 { get; set; }
            public bool Level3 { get; set; }

        }

        private LevelCs purityCs;

        public LevelCs _PurityCs
        {
            get { return purityCs; }
            set { purityCs = value;OnPropertyChanged(); }
        }

        public RelayCommand<GridView> ScrlLoad { get; private set; }


        private LevelCs Categorycs;

        public LevelCs _CategoryCs
        {
            get { return Categorycs; }
            set { Categorycs = value;OnPropertyChanged(); }
        }


        private bool ProCess;

        public bool _ProCess
        {
            get { return ProCess; }
            set { ProCess = value;OnPropertyChanged(); }
        }
        private string pagetitle;

        public string PageTitle
        {
            get { return pagetitle; }
            set { pagetitle = value; OnPropertyChanged(); }
        }

        public async void LinkClick()
        {
            SV.ViewChanged -= SV_ViewChanged;           //这里卸载一下滚动事件，防止超载加载
            _ProCess = true;
            if(pagesize < maxpagesize)
            {
                pagesize++;
                var results = await tools.GetSearchWallpaperString(Sorting, topRange.D1, "", "", CategoryString, PruityString, Order, pagesize, colors.None);
                foreach (var result in results.WallpaperList)
                {
                    Walls.WallpaperList.Add(result);
                }
                PageTitle = $"已经加载{pagesize}页,最后一页为{maxpagesize}";
            }
            else if(pagesize == maxpagesize)
            {
                ContentDialog dialog = new ContentDialog();
                dialog.PrimaryButtonText = "了解";
                dialog.Title = "温馨提示";
                TextBlock text =   new TextBlock() { Text ="别拉了别拉了，到底了。" };
                dialog.Content = text;
                await dialog.ShowAsync();
            }
            _ProCess = false;
            SV.ViewChanged += SV_ViewChanged;           //完了之后再加上
        }

        public void Changed(object sender, SelectionChangedEventArgs e)
        {
            Wallpaper wall = (Wallpaper)(sender as GridView).SelectedItem;
            if (wall != null)
            {
                Frame rootFrame = Window.Current.Content as Frame;      //顶部Frame
                rootFrame.Navigate(typeof(WallpaperPage), wall, new DrillInNavigationTransitionInfo());

            }
        }
    }
}
