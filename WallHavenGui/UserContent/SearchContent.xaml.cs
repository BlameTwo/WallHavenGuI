using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using WallEventGUI.UserContent.ContentVM;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.UserContent
{
    public sealed partial class SearchContent : UserControl
    {
        SearchContentVM vm = new SearchContentVM();
        Frame rootFrame;
        public SearchContent()
        {
            this.InitializeComponent();
            this.DataContext = vm;
            rootFrame = Window.Current.Content as Frame;
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += SearchPage_CloseRequested;
        }

        private void SearchPage_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            //这个是应用程序关闭后得事件，只不过不包括强制关闭（例如任务栏关闭等）
            vm.Save();
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if(args != null)
            {
                SearchButton.Text = args.SelectedItem.ToString();
            }
        }




        public string Puritys
        {
            get { return (string)GetValue(_PurityProperty); }
            set { SetValue(_PurityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Purity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _PurityProperty =
            DependencyProperty.Register("Puritys", typeof(string), typeof(SearchContent), new PropertyMetadata(null,new PropertyChangedCallback(updatepurity)));

        private static void updatepurity(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //通过依赖属性更新数据，如果搜索内容以及字段不为空，那么就执行搜索
            var ob = (d as SearchContent);
            if (!string.IsNullOrWhiteSpace(e.NewValue as string))
            {
                switch (e.NewValue)
                {
                    case "100":
                        ob.vm._PurityCS = new LevelCs() { Level1 = true };
                        break;
                    case "010":
                        ob.vm._PurityCS = new LevelCs() { Level2 = true };
                        break;
                    case "001":
                        ob.vm._PurityCS = new LevelCs() { Level3 = true };
                        break;
                }
            }
        }




        public bool IsSave
        {
            get { return (bool)GetValue(IsSaveProperty); }
            set { SetValue(IsSaveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSave.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSaveProperty =
            DependencyProperty.Register("IsSave", typeof(bool), typeof(SearchContent), new PropertyMetadata(null,new PropertyChangedCallback(back)));

        private static void back(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool bl = e.NewValue is bool;
            if (bl)
            {
                SearchContent content = d as SearchContent;
                //属性更改为真时则调用保存
                content.vm.Save();
            }
        }

        public string KeyWord
        {
            get { return (string)GetValue(KeyWordProperty); }
            set { SetValue(KeyWordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyWord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyWordProperty =
            DependencyProperty.Register("KeyWord", typeof(string), typeof(SearchContent), new PropertyMetadata(null,new PropertyChangedCallback(updata)));

        private static void updata(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //通过依赖属性更新数据，如果搜索内容以及字段不为空，那么就执行搜索
            var ob = (d as SearchContent);
            if (!string.IsNullOrWhiteSpace(e.NewValue as string))
            {
                ob.vm._SearchText = e.NewValue.ToString();
                ob.vm.search();
            }
        }

        
    }
}
