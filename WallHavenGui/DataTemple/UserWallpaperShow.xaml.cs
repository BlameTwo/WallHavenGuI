using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.WallHavenTools;
using WallHavenGui.Account.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.DataTemple
{
    public sealed partial class UserWallpaperShow : UserControl
    {
        public UserWallpaperShow()
        {
            this.InitializeComponent();
            this.Loaded += UserWallpaperShow_Loaded;
        }

        private void UserWallpaperShow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public LikeWallpaper MyData
        {
            get { return (LikeWallpaper)GetValue(MyDataProperty); }
            set { SetValue(MyDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyDataProperty =
            DependencyProperty.Register(
                "MyData",
                typeof(LikeWallpaper),
                typeof(UserWallpaperShow),
                new PropertyMetadata(null));
    }
}
