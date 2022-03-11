using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static WallEventGUI.Model.UIModel;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WallHavenGui.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        Frame rootFrame;
        public SearchPage()
        {
            this.InitializeComponent();
            rootFrame =  Window.Current.Content as Frame ;
            
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if ((e.Parameter as SearchArgs) == null)
            {
                BackButton.Visibility = Visibility.Collapsed;
                return;
            }

            if((e.Parameter as SearchArgs).NewWindow)
            {
                BackButton.Visibility = Visibility.Collapsed;
            }
            var arg = e.Parameter as SearchArgs;
            searchcontent.KeyWord = arg.KeyWord;
            Window.Current.SetTitleBar(AppBar);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            WallHevenSettingResource home = new WallHevenSettingResource();
            if (System.Convert.ToBoolean( home.SettingGetConfig(AppSettingArgs.SearchSave)))
            {
                searchcontent.IsSave = true;
            }
            else
            {

                searchcontent.IsSave = false;
            }
            base.OnNavigatedFrom(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (rootFrame.CanGoBack)
                rootFrame.GoBack();
        }
    }
}
