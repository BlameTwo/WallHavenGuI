using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallHavenGui.Model;
using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml;

namespace WallHavenGui.ViewModel
{
    public class DownloadPageVM: ObservableObject
    {
        public DownloadPageVM()
        {
            _MyList = new ObservableCollection<DownOption>();
        }

        private ObservableCollection<DownOption> MyList;

        public ObservableCollection<DownOption> _MyList
        {
            get { return MyList; }
            set { MyList = value; OnPropertyChanged(); }
        }


        public async void Loaded()
        {
            var ls = await BackgroundDownloader.GetCurrentDownloadsForTransferGroupAsync(DownloadHelper.group);
            //获取未下载完的壁纸
            int a = ls.Count;

            foreach (var item in ls)
            {
                DownOption option = new DownOption();
                double process = 0;
                try
                {
                    process = (item.Progress.TotalBytesToReceive / item.Progress.BytesReceived) * 100;
                }
                catch (Exception)
                {
                    process = 100;
                }
                option.Name = item.RequestedUri.ToString();
                _MyList.Add(option);
            }
        }
    }
}
