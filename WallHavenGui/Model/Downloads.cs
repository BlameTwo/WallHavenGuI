using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace WallHavenGui.Model
{
    public class Downloads
    {

        public static async Task<string> SaveImage(string imageUri, StorageFolder folder,string localfile)
        {
            try
            {
                BackgroundDownloader backgroundDownload = new BackgroundDownloader();
                StorageFile newFile = await folder.CreateFileAsync(localfile, CreationCollisionOption.OpenIfExists);
                Uri uri = new Uri(imageUri);
                DownloadOperation download = backgroundDownload.CreateDownload(uri, newFile);
                await download.StartAsync();
                return "下载成功！";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
