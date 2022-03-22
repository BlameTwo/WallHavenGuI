using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallHavenGui.Model;
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
        FrameNavigationOptions navOptions = new FrameNavigationOptions();

        
            Type typepage = null;

        private async void Naviga_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
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
            if (DownloadTask.Wait() == false)
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "检查到有下载任务未完成，是否跳转页面？";
                dialog.PrimaryButtonText = "无视任务";
                dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
                dialog.CloseButtonText = "取消";
                await dialog.ShowAsync();
            }
            else
            {
                MyFrame.Navigate(typepage, null, new DrillInNavigationTransitionInfo());
            }
        }

       

        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
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
            if(MyFrame.SourcePageType == typeof(HelpPage))
            {
                Naviga.SelectedItem = HelpPage;
            }
            if(MyFrame.SourcePageType == typeof(DocumentPage))
            {
                Naviga.SelectedItem = DocumentPage;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(AppBar);
            base.OnNavigatedTo(e);
        }
    }
}
