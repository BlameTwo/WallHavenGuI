using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallHavenGui.Pages;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace WallEventGUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Naviga.BackRequested += Naviga_BackRequested;
        }

        private void Naviga_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (MyFrame.CanGoBack == true)
                Naviga.SelectedItem = sender.SelectedItem;
            MyFrame.GoBack();
        }


        private void Naviga_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();

            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            Type typepage = null;
            if (args.SelectedItem == HomeSml)
            {
                typepage = typeof(HomePage);
            }
            if (args.SelectedItem == SearchSml)
            {
                typepage = typeof(SearchPage);
            }
            if (args.SelectedItem == LikesPage)
                typepage = typeof(LikesPage);
            if (args.SelectedItem == DocumentPage)
                typepage = typeof(DocumentPage);
            if (args.IsSettingsSelected == true)
                typepage = typeof(NewSettingPage);

            if(args.SelectedItem == HelpPage)
                typepage = typeof(HelpPage);
            MyFrame.Navigate(typepage, null, new DrillInNavigationTransitionInfo());
        }

        private void MyFrame_Navigated(object sender, NavigationEventArgs e)
        {
            //当Frame产生后退后，操作标题以及导航
            Naviga.IsBackEnabled = MyFrame.CanGoBack;
            if (MyFrame.SourcePageType == typeof(HomePage))
            {
                Naviga.SelectedItem = HomeSml;
            }
            if (MyFrame.SourcePageType == typeof(SearchPage))
            {
                Naviga.SelectedItem = SearchSml;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(AppBar);
            base.OnNavigatedTo(e);
        }
    }
}
