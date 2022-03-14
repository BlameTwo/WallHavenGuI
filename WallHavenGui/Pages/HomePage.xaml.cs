using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Markup;
using Windows.Storage;
using WallHavenGui.Model;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WallHavenGui.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        HomeViewModel vm = new HomeViewModel();
        public HomePage()
        {
            this.InitializeComponent();
            this.DataContext = vm;
            this.SizeChanged += HomePage_SizeChanged;   
            NavigationCacheMode = NavigationCacheMode.Required;
            
        }

        

        private void HomePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.SizeChanged -= HomePage_SizeChanged;
            
            base.OnNavigatingFrom(e);
        }
    }
}
