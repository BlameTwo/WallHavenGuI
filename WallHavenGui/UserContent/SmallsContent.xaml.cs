using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WallEventGUI.WallHavenTools;
using WallHavenGui.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.UserContent
{
    /// <summary>
    /// 这是对话框的
    /// </summary>
    public sealed partial class SmallsContent : UserControl
    {
        public SmallsContent()
        {
            this.InitializeComponent();
            IsSelect = false;
        }

        List<Wallpaper> dataList = new List<Wallpaper>();
        private async void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string value =  (DataList.SelectedItem as BigClass).Name;
            var list = dataList;
            bool result = await SaveImage(value, list);
            WallXml wallxml2 = new WallXml();
            if (result)
            {
                IsSelect = true;
            }
        }





        async Task<bool> SaveImage(string value, List<Wallpaper> walls)
        {
            return await Task.Run(() =>
            {
                WallXml wallxml2 = new WallXml();
                wallxml2.AddImageedId(walls, value);
                return true;
            });
        }

        public List<Wallpaper> MyData
        {
            get { return (List<Wallpaper>)GetValue(MyDataProperty); }
            set { SetValue(MyDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyDataProperty =
            DependencyProperty.Register("MyData", typeof(List<Wallpaper>), typeof(SmallsContent), new PropertyMetadata(null,back));


        public bool IsSelect
        {
            get { return (bool)GetValue(IsSelectProperty); }
            set { SetValue(IsSelectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectProperty =
            DependencyProperty.Register("IsSelect", typeof(bool), typeof(SmallsContent), new PropertyMetadata(null,back1));

        private async  static void back1(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (System.Convert.ToBoolean(e.NewValue) == true)
            {
                var elment = d as SmallsContent;
                if (elment.Dialog != null)        //当依赖属性为真的时候，就执行隐藏
                {
                    elment.Dialog.Hide();
                }
                else
                {
                    await elment.Dialog.ShowAsync();
                }
            }
        }

        public string FatherData
        {
            get { return (string)GetValue(FatherDataProperty); }
            set { SetValue(FatherDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FatherData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FatherDataProperty =
            DependencyProperty.Register("FatherData", typeof(string), typeof(SmallsContent), new PropertyMetadata(null,back3));

        private async static void back3(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var elment = d as SmallsContent;
            WallXml wallxml = new WallXml();
            var list  = await wallxml.GetBigImage();
            if (list == null)
            {
                elment.Dialog.Hide();
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name.ToString() == e.NewValue.ToString())
                    list.RemoveAt(i);
            }
            if(list.Count >= 1)         //大于和等于1的情况下才会正常显示！
            {
                elment.DataList.ItemsSource = list;
            }
            else
            {
                elment.Dialog.Hide();
            }
        }

        public ContentDialog Dialog
        {
            get { return (ContentDialog)GetValue(DialogProperty); }
            set { SetValue(DialogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dialog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogProperty =
            DependencyProperty.Register("Dialog", typeof(ContentDialog), typeof(SmallsContent), new PropertyMetadata(0));






        /// <summary>
        /// 回调方法
        /// </summary>
        /// <param name="d">控件本身</param>
        /// <param name="e">被赋值的新值参数</param>
        /// <exception cref="NotImplementedException"></exception>
        private static void back(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var result =  d as SmallsContent;
            if(e.NewValue != null)
            {
                result.dataList = e.NewValue as List<Wallpaper>;
            }
        }
    }
}
