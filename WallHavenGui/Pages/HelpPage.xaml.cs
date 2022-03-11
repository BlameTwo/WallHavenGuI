using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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
    public sealed partial class HelpPage : Page
    {
        ObservableCollection<GitClass> _gitClasses = new ObservableCollection<GitClass>();
        public HelpPage()
        {
            this.InitializeComponent();
            _gitClasses.Add(new GitClass()
            {
                Name = "Microsoft.Toolkit.Uwp.UI.Controls",
                Url = "https://github.com/CommunityToolkit/WindowsCommunityToolkit"
                ,By="使用社区控件呈现更好的效果。"
            });


            _gitClasses.Add(new GitClass()
            {
                Name = "Microsoft.Tookit.Mvvm",
                Url = "https://github.com/CommunityToolkit/WindowsCommunityToolkit",
                By = "构造标准的Mvvm模式所需要，本项目使用的是耦合和解耦两种方式开发"
            });
            _gitClasses.Add(new GitClass()
            {
                Name = "Microsoft.UI.Xaml",
                Url = " https://github.com/microsoft/microsoft-ui-xaml",
                By = "此程序的UI层展现"
            });
            _gitClasses.Add(new GitClass()
            {
                Name = "Microsoft.Toolkit.Uwp.UI.Controls",
                Url = "https://github.com/CommunityToolkit/WindowsCommunityToolkit"
                ,
                By = "使用社区控件呈现更好的效果"
            });
            _gitClasses.Add(new GitClass()
            {
                Name = "NewTonsoft.Json",
                Url = "https://www.newtonsoft.com/json"
                ,
                By = "解析从Webapi中获得的内容"
            });
            this.Loaded += HelpPage_Loaded;
        }

        private void HelpPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(_gitClasses.Count > 0)
            {
                GitList.ItemsSource = _gitClasses;
            }
        }

        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://wallhaven.cc/"));
        }

        private async void GitList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GitClass gitClass = (GitClass)GitList.SelectedItem;
            if(gitClass != null)
            {
                await Launcher.LaunchUriAsync(new Uri(gitClass.Url));
                GitList.SelectedIndex = -1;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var t = new FolderLauncherOptions();
            StorageFolder folider = Windows.Storage.ApplicationData.Current.LocalFolder;
            await Launcher.LaunchFolderAsync(folider, t);
        }
    }
}
