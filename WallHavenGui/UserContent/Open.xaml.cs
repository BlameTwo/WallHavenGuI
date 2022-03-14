using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallEventGUI.Model;
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
    public sealed partial class Open : UserControl
    {
        public Open()
        {
            this.InitializeComponent();
            bool result = System.Convert.ToBoolean(home.SettingGetConfig(AppSettingArgs.Is18));
            if (result)
            {
                OpenOrClear.Content = "关闭18+";
                return;
            }
            OpenOrClear.Content = "打开18+";
        }


        WallHevenSettingResource home = new WallHevenSettingResource();




        public ContentDialog Dialog
        {
            get { return (ContentDialog)GetValue(DialogProperty); }
            set { SetValue(DialogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dialog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogProperty =
            DependencyProperty.Register("Dialog", typeof(ContentDialog), typeof(Open), new PropertyMetadata(0));



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Open), new PropertyMetadata(null,backk));

        private static void backk(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //如果为真则关闭
            var elment = (Open)d;
            if(System.Convert.ToBoolean(e.NewValue) == true)
            {
                elment.Dialog.Hide();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(OpenOrClear.Content.ToString() == "打开18+")
            {
                if (!string.IsNullOrWhiteSpace(KeyText.Text))
                {
                    if (AppSettingArgs.Exites18(KeyText.Text))
                    {
                        IsOpen = true;
                    }
                    else
                    {

                        Dialog.Title = "密钥错误！";
                    }
                }
                
            }else if (OpenOrClear.Content.ToString() == "关闭18+")
            {
                home.SettingSetConfig(AppSettingArgs.Is18, false.ToString());
                IsOpen = true;
            }
            
        }
    }
}
