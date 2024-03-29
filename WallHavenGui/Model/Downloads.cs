﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.WallHavenTools;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using static System.Net.Mime.MediaTypeNames;

namespace WallHavenGui.Model
{


    public class DownLoadArgs
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public StorageFolder Path { get; set; }
    }


    public class DownOption
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public double process { get; set; }
    }

    public static class Downloads
    {
        public static BackgroundTransferGroup group = BackgroundTransferGroup.CreateGroup("WallDown");//下载组，方便管理

        public async static Task AddDownload(DownLoadArgs args)
        {
            await SaveImage(args.Url, args.Path, args.Name);
        } 


        public static async Task<string> SaveImage(string imageUri, StorageFolder folder,string localfile)
        {
            
            try
            {
                BackgroundDownloader backgroundDownload = new BackgroundDownloader();
                backgroundDownload.TransferGroup = group;
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
