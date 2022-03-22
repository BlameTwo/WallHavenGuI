using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WallHavenGui.Account.Model;
using WallHavenGui.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WallHavenGui.Account.AccountPage
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Like : Page
    {
        UserLikeVM vm = new UserLikeVM();
        public Like()
        {
            this.InitializeComponent();
            //this.Loaded += Like_Loaded;
            this.DataContext = vm;
        }
        //WebMoreApi api = new WebMoreApi();
        //ScrollViewer SV = new ScrollViewer();

        //private async void Like_Loaded(object sender, RoutedEventArgs e)
        //{

        //    SV = (VisualTreeHelper.GetChild(Mydata, 0) as Border).Child as ScrollViewer;
        //    SV.ViewChanged += SV_ViewChanged;
        //    if (Commections != null)
        //    {
        //        string url = Commections.Url;
        //        Mydata.ItemsSource = await api.GetLists(url+$"?page={page}");
        //        page++;
        //    }
        //}
        //private async void SV_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        //{
        //    var sv = sender as ScrollViewer;
        //    var flage = sv.VerticalOffset + sv.ViewportHeight;          //已经滚动的长度
        //    if (sv.ExtentHeight - flage < 5 && sv.ViewportHeight != 0)
        //    {
        //       await LinkClick();
        //    }
        //}

        //int page = 1;
        //private async Task LinkClick()
        //{
        //    SV.ViewChanged -= SV_ViewChanged;
        //    if(Mydata.Items.Count < int.Parse(Commections.ImageCount))
        //    {
        //        var list =  await api.GetLists(Commections.Url+$"?page={page}");
        //        ObservableCollection<LikeWallpaper> list2= new ObservableCollection<LikeWallpaper>();
        //        foreach (var item in Mydata.Items)
        //        {
        //            var mod = item as LikeWallpaper;
        //            list2.Add(mod);
        //        }
        //        foreach (var item in list)
        //            list2.Add(item);
        //        Mydata.ItemsSource = list2;
        //        page++;
        //    }
        //    SV.ViewChanged += SV_ViewChanged;
        //}

        //MyCommections Commections { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm.Commections = e.Parameter as MyCommections;
            base.OnNavigatedTo(e);
        }

        
    }
}
