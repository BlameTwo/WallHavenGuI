using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WallHavenGui.UserContent
{
    public sealed partial class AddBigImage : UserControl
    {

        XmlViewModel vm = new XmlViewModel();
        public AddBigImage()
        {
            this.InitializeComponent();
        }




        public bool IsShow
        {
            get { return (bool)GetValue(IsShowProperty); }
            set { SetValue(IsShowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowProperty =
            DependencyProperty.Register("IsShow", typeof(bool), typeof(AddBigImage), new PropertyMetadata(null,IsShowChanged));

        private static void IsShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var elment = d as AddBigImage;
            if(System.Convert.ToBoolean(e.NewValue))
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
            DependencyProperty.Register("Dialog", typeof(ContentDialog), typeof(AddBigImage), new PropertyMetadata(null,back));

        private static void back(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        
    }
}
