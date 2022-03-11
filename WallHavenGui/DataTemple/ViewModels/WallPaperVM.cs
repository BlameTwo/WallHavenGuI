using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.WallHavenTools;

namespace WallHavenGui.DataTemple.ViewModels
{
    public class WallPaperVM: ObservableObject
    {
        public WallPaperVM()
        {
            
        }

        private thumbs thumbs;

        public thumbs Thumbs
        {
            get { return thumbs; }
            set { thumbs = value; OnPropertyChanged(); }
        }


        private string Resolution;

        public string resolution
        {
            get { return Resolution; }
            set { Resolution = value;OnPropertyChanged(); }
        }

        private string Favorites;

        public string favorites
        {
            get { return Favorites; }
            set { Favorites = value;OnPropertyChanged(); }
        }

        private string views;

        public string Views
        {
            get { return views; }
            set { views = value; OnPropertyChanged(); }
        }

    }
}
