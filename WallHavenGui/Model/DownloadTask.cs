using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using static WallHavenGui.Model.Downloads;

namespace WallHavenGui.Model
{
    public static class DownloadTask
    {
        public static BackgroundTransferGroup group = BackgroundTransferGroup.CreateGroup("WallDown");//下载组，方便管理
        
        public static List<Task<string>> DownList = new List<Task<string>>();
        public static ObservableCollection<Task<DownLoadArgs>> DownLoadArgs = new ObservableCollection<Task<DownLoadArgs>>();

        public  static void AddAsync(Task<string> action)
        {
            DownList.Add(Task.Run(()=>action));
            
        }


        public  static void  AddAsync(Task<DownLoadArgs> args)
        {
            DownLoadArgs.Add(Task.Run(async () => await args));
        }

        public static bool Wait()
        {
            foreach (var item in DownList)
            {
                if (!(item.Status == TaskStatus.RanToCompletion))
                {
                    return false;
                }
            }
            return true;
        }


    }
}
