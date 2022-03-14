using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.WallHavenTools;
using WallHavenGui.Model;
using WallHavenGui.UserContent;
using Windows.UI.Xaml;

namespace WallHavenGui.ViewModel
{
    public class XmlViewModel: ObservableObject
    {
        WallXml wallxml = new WallXml();

        private AddBigImage window;

        public AddBigImage Window
        {
            get { return window; }
            set { window = value; OnPropertyChanged(); }
        }

        private string bigname;

        public string BigName
        {
            get { return bigname; }
            set { bigname = value; OnPropertyChanged(); }
        }


        public XmlViewModel()
        {
            CreateBigImage = new RelayCommand(() => createbieimage());
            AddBigImage = new RelayCommand<AddBigImage>((name) => exit(name));
        }

        private void exit(AddBigImage name)
        {
            Window = name;
        }

        private async void createbieimage()
        {
            if(!string.IsNullOrWhiteSpace(BigName))
            {
                if(!await wallxml.BigExits(BigName))
                {
                    await wallxml.CreateSmalls(BigName);
                    Window.IsShow = true;
                }
                Window.Dialog.Title = "已存在该收藏夹";
            }
        }
        public RelayCommand CreateBigImage { get; set; }
        public RelayCommand<AddBigImage> AddBigImage { get; set; }


        public void Loaded(object sender, RoutedEventArgs e)
        {
            Window = sender as AddBigImage;
        }
    }
}
