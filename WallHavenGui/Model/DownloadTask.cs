using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WallHavenGui.Model
{
    public static class DownloadTask
    {

        public static List<Task<string>> DownList = new List<Task<string>>();

        public  static void AddAsync(Task<string> action)
        {
            DownList.Add(Task.Run(()=>action));
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
