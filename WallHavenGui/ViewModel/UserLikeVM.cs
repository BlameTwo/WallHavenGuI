using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.Model;
using WallEventGUI.WallHavenTools;
using WallHavenGui.Account;
using WallHavenGui.Account.Model;
using WallHavenGui.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace WallHavenGui.ViewModel
{
    public class UserLikeVM: ObservableObject
    {
        WebMoreApi api = new WebMoreApi();
        ScrollViewer SV = new ScrollViewer();

        public RelayCommand<AdaptiveGridView> ScrlLoad { get; private set; }

        int page = 1;
        public UserLikeVM()
        {
            ScrlLoad = new RelayCommand<AdaptiveGridView>((se) => scrool(se));
            MyData = new ObservableCollection<LikeWallpaper>();
        }
        WallHevenSettingResource home = new WallHevenSettingResource();
        public async void Mydata_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mylike = e.AddedItems[0] as LikeWallpaper;
            
            if (string.IsNullOrWhiteSpace(mylike.PicUrl)&&!System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.Is18)))           //可以打开
            {
                return;
            }
            Tools tools = new Tools();
            tools.OpenKey = true;
            tools.Key = home.SettingGetConfig(AppSettingArgs.OpenKey);
            var value = await tools.GetWallpaperAsync(mylike.Id);
            Frame frame = Window.Current.Content as Frame;      //顶部Frame
            frame.Navigate(typeof(WallpaperPage), value);
        }

        private ObservableCollection<LikeWallpaper> myData;

        public ObservableCollection<LikeWallpaper> MyData
        {
            get { return myData; }
            set { myData = value; OnPropertyChanged(); }
        }


        public WallHavenGui.Account.Model.MyCommections Commections { get; set; }

        



        private async void scrool(GridView se)
        {
            var listview = (VisualTreeHelper.GetChild(se, 0) as Border).Child as ScrollViewer;
            SV = listview;
            SV.ViewChanged += SV_ViewChanged;
            if (Commections != null)
            {
                string url = Commections.Url;
                var list =  await api.GetLists(url + $"?page={page}");
                foreach (var item in list)
                {
                    MyData.Add(item);
                }
                page++;
            }
        }

        private async void SV_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            var flage = sv.VerticalOffset + sv.ViewportHeight;          //已经滚动的长度
            if (sv.ExtentHeight - flage < 5 && sv.ViewportHeight != 0)
            {
                await LinkClick();
            }
        }

        private async Task LinkClick()
        {
            SV.ViewChanged -= SV_ViewChanged;
            if (MyData.Count < int.Parse(Commections.ImageCount))
            {
                var list = await api.GetLists(Commections.Url + $"?page={page}");
                foreach (var item in list)
                {
                    MyData.Add(item);
                }
                page++;
            }
            SV.ViewChanged += SV_ViewChanged;
        }
    }
}
