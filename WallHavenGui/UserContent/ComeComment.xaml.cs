using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WallHavenGui.Account.Model;
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
    public sealed partial class ComeComment : UserControl
    {
        public ComeComment()
        {
            this.InitializeComponent();
            this.Loaded += ComeComment_Loaded;

        }

        private void ComeComment_Loaded(object sender, RoutedEventArgs e)
        {
            IControl.ItemsSource = MyData.comeComment;
            Count.Text = MyData.comeComment.Count.ToString();
        }

        public UserComment MyData { get; set; }





        public ContentDialog Dialog
        {
            get { return (ContentDialog)GetValue(DialogProperty); }
            set { SetValue(DialogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dialog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogProperty =
            DependencyProperty.Register("Dialog", typeof(ContentDialog), typeof(ComeComment), new PropertyMetadata(0));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dialog.Hide();
        }
    }
}
