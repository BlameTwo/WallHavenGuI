using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WallEventGUI.WallHavenTools;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WallHavenGui.Model
{
    /// <summary>
    /// 收藏夹主要方法
    /// </summary>
    public class WallXml : WallXmlClass
    {
        /// <summary>
        /// 这里是获取
        /// </summary>
        /// <returns></returns>
        XmlDocument GetLikeImages()
        {
            XmlDocument xmldoc = new XmlDocument();

            return xmldoc;
        }

        /// <summary>
        /// 创建头部文件，并保存文件
        /// </summary>
        /// <returns></returns>
        public XmlDocument CreateNewHeader()
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "");//xml文档的声明部分
            EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
            doc.AppendChild(declaration);
            XmlElement el = doc.CreateElement("WallpaperBigs");
            el.SetAttribute("User", deviceInfo.FriendlyName);
            doc.AppendChild(el);
            return doc;
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="flage"></param>
        /// <param name="id"></param>
        /// <param name="Name"></param>
        public void SaveDefaultImageId(bool flage, List<Wallpaper> id, string Name)
        {
            if (!flage)
                CreateImageedId(id, Name);
            else
                AddImageedId(id, Name);
        }

        /// <summary>
        /// 插入文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Name"></param>
        public async void CreateImageedId(List<Wallpaper> id, string Name)
        {
            StorageFile file = await folider.CreateFileAsync("MyImages.xml");
            var xmldoc = CreateNewHeader();
            var bigs = (XmlElement)xmldoc.SelectSingleNode("WallpaperBigs");
            var small = xmldoc.CreateElement("WallpaperSmalls");
            small.SetAttribute("Name", "Default");
            foreach (var item in id)
            {
                WallXml wallxml = new WallXml();
                var wallpaper = WriteImage(item);     //这里返回的是一个string字符串，也就是处理完毕的XmlElment
                XmlDocument xmldoc2 = new XmlDocument();
                xmldoc2.LoadXml(wallpaper);
                var list = xmldoc2.SelectSingleNode("Wallpaper");
                var result = small.OwnerDocument.ImportNode(list, true);
                small.AppendChild(result);
            }
            bigs.AppendChild(small);
            xmldoc.Save(file.Path);
        }

        /// <summary>
        /// 插入壁纸
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="Name"></param>
        public async void AddImageedId(List<Wallpaper> lists, string Name)
        {
            StorageFile file = await folider.GetFileAsync("MyImages.xml");
            var xmldoc = new XmlDocument();
            WallFile wallfile = new WallFile();
            xmldoc.LoadXml(await wallfile.GetXmlString());
            var xmlel = (XmlElement)xmldoc.SelectSingleNode("WallpaperBigs");
            var xmlel2 = xmlel.SelectNodes("WallpaperSmalls");
            XmlElement xmlelment = null;
            foreach (XmlNode item in xmlel2)
            {
                XmlElement item1 = (XmlElement)item;
                string txt = item.Attributes["Name"].Value;
                if (txt == Name)
                {
                    xmlelment = (XmlElement)item;
                }
            }
            foreach (var item in lists)
            {
                if(await SmallExits(item.id, Name))
                {
                    continue;
                }
                else
                {
                    string xmlstring = WriteImage(item);
                    XmlDocument xmldoc2 = new XmlDocument();
                    xmldoc2.LoadXml(xmlstring);
                    XmlElement newimage = (XmlElement)xmldoc2.SelectSingleNode("Wallpaper");
                    var result = xmlelment.OwnerDocument.ImportNode(newimage, true);            //首先使用此方法将两个文档连接起来，才可以互相添加
                    xmlelment.AppendChild(result);
                }
            }
            Thread.Sleep(200);
            xmldoc.Save(file.Path);
        }

        /// <summary>
        /// 创建收藏夹
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<bool> CreateSmalls(string Name)
        {
            WallFile wallfile = new WallFile();
            if (await wallfile.FileExites())
            {
                var fold = await folider.GetFileAsync("MyImages.xml");
                EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
                var xmldoc = new XmlDocument();
                xmldoc.LoadXml(await wallfile.GetXmlString());
                XmlElement xmlelment = xmldoc.CreateElement("WallpaperSmalls");
                xmlelment.SetAttribute("Name", Name);
                var node = xmldoc.SelectSingleNode($"//WallpaperBigs[@User='{deviceInfo.FriendlyName}']");
                if (node != null)
                {
                    node.AppendChild(xmlelment);
                    xmldoc.Save(fold.Path);
                    return true;
                }
            }
            else
            {
                StorageFile file = await folider.CreateFileAsync("MyImages.xml");
                var xmldoc2 = CreateNewHeader();
                var bigs = (XmlElement)xmldoc2.SelectSingleNode("WallpaperBigs");
                var small = xmldoc2.CreateElement("WallpaperSmalls");
                small.SetAttribute("Name", Name);
                bigs.AppendChild(small);
                xmldoc2.AppendChild(bigs);
                xmldoc2.Save(file.Path);
                return true;
            }
            
            return false;
        }


        /// <summary>
        /// 批量删除壁纸
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="WallpaperList"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImage(string Name, List<Wallpaper> WallpaperList)
        {
            var fold = await folider.GetFileAsync("MyImages.xml");
            var xmldoc = new XmlDocument();
            WallFile wallfile = new WallFile();
            xmldoc.LoadXml(await wallfile.GetXmlString());
            XmlElement small = (XmlElement)xmldoc.SelectSingleNode($"//WallpaperSmalls[@Name='{Name}']");
            var list = small.SelectNodes("Wallpaper");
            if (WallpaperList.Count == 1)
            {
                foreach (XmlNode item in list)
                {
                    string id = item.SelectSingleNode("./id").InnerText;
                    if(id == WallpaperList[0].id)
                    {
                        small.RemoveChild(item);
                    }
                }
                xmldoc.Save(fold.Path);
                return true;            //单个壁纸删除
            }
            foreach (XmlNode item in list)
            {
                string id = item.SelectSingleNode("./id").InnerText;        //使用XPath选择id
                foreach (var item2 in WallpaperList)
                {
                    //下面一句代码为文档中的id，接下来开始遍历
                    if(item2.id == id)
                    {
                        small.RemoveChild(item);
                    }
                }
            }
            xmldoc.Save(fold.Path);
            return true;        //多个壁纸删除
        }


        /// <summary>
        /// 删除收藏夹
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<bool> DeleteSmall(string Name)
        {
            var fold = await folider.GetFileAsync("MyImages.xml");
            var xmldoc = new XmlDocument();
            WallFile wallfile = new WallFile();
            xmldoc.LoadXml(await wallfile.GetXmlString());
            var bigs = xmldoc.SelectSingleNode($"WallpaperBigs");
            var small = bigs.SelectSingleNode($"./WallpaperSmalls[@Name='{Name}']");
            bigs.RemoveChild(small);
            xmldoc.Save(fold.Path);
            return true;
        }

        /// <summary>
        /// 序列化Wallpaper返回xml文本
        /// </summary>
        /// <param name="wallpaper"></param>
        /// <returns></returns>
        string WriteImage(Wallpaper wallpaper)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(wallpaper.GetType());
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.OmitXmlDeclaration = true;
                xws.Encoding = Encoding.UTF8;
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add("Wallpaper", wallpaper.id);
                serializer.Serialize(sw, wallpaper,xmlSerializerNamespaces);
                sw.Close();
                string a = sw.ToString();
                return a;
            }
        }


        public async Task<bool> BigExits(string Name)
        {
            WallFile wallfile = new WallFile();
            string xmlstr = await wallfile.GetXmlString();
            XmlDocument xmldoc = new XmlDocument(); 
            xmldoc.LoadXml(xmlstr);
            var elment = xmldoc.SelectSingleNode($"//WallpaperSmalls[@Name='{Name}']");
            if(elment != null)
            {
                return true;
            }
            return false;
        }


        public async Task<bool> SmallExits(string id,string Name)
        {
            WallFile wallfile = new WallFile();
            string xmlstr = await wallfile.GetXmlString();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xmlstr);
            var small = xmldoc.SelectSingleNode($"//WallpaperSmalls[@Name='{Name}']");
            foreach (var item in small.SelectNodes("./Wallpaper"))
            {
                XmlElement xmlel = (XmlElement)item;
                string txt = xmlel.SelectSingleNode("./id").InnerText;
                if(txt == id)
                {
                    return true;
                }
                else
                {
                    continue;
                }
                
            }
            return false;
        }


        /// <summary>
        /// 获取全部收藏夹
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<BigClass>> GetBigImage()
        {
            ObservableCollection<BigClass> result = new ObservableCollection<BigClass>();
            WallFile wallfile = new WallFile();
            XmlDocument xmldoc = new XmlDocument();
            string xmlstr = await wallfile.GetXmlString();
            if (xmlstr == null)
                return null;
            xmldoc.LoadXml(xmlstr);
            var node = xmldoc.SelectSingleNode("WallpaperBigs");
            var nodes = node.SelectNodes("WallpaperSmalls ");
            foreach (XmlNode node1 in nodes)
            {
                XmlElement xmlel = (XmlElement)node1;
                BigClass bigClass = new BigClass()
                {
                    Name = xmlel.GetAttribute("Name")
                };
                result.Add(bigClass);
            }
            return result;
        }




        /// <summary>
        /// 获取指定收藏夹
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<Wallpaper>> GetSmallImage(string name,int page,  int maxpage)
        {
            ObservableCollection<Wallpaper> list = new ObservableCollection<Wallpaper>();
            WallFile wallfile = new WallFile();
            XmlDocument xmldoc = new XmlDocument(); xmldoc.LoadXml(await wallfile.GetXmlString());
            var bigs = xmldoc.SelectSingleNode("WallpaperBigs");
            var small = bigs.SelectNodes("WallpaperSmalls");
            XmlElement select = null;
            foreach (XmlNode index in small)
            {
                var xmlel = index;
                if (xmlel.Attributes["Name"].Value == name)
                {
                    select = (XmlElement)index;
                }
            }
            if(select != null)
            {
                XmlNodeList nodelist = select.SelectNodes("Wallpaper");
                if(maxpage == 999 && page == 999 &&nodelist.Count != 0)       //不限制读取
                {
                    foreach (XmlNode node in nodelist)
                    {
                        string xmlstr = node.OuterXml;
                        list.Add(XmlGetWallpaper(xmlstr));
                    }
                    return list;
                }
                else if(nodelist.Count != 0)
                {
                    for (int i = page; i < maxpage; i++)
                    {
                        if(nodelist[i]!= null)
                        {
                            string xmlstr = nodelist[i].OuterXml;
                            list.Add(XmlGetWallpaper(xmlstr));
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="xmlstr"></param>
        /// <returns></returns>
        Wallpaper XmlGetWallpaper(string xmlstr)
        {
            using (StringReader sr = new StringReader(xmlstr))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Wallpaper));
                Wallpaper wallpaper = (Wallpaper)serializer.Deserialize(sr);
                return wallpaper;
            }

        }

    }


    


    public class WallFile : WallXmlClass
    {
        /// <summary>
        /// 获取已经存在的收藏夹xml文本
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetXmlString()
        {
            if (await FileExites() == true)
            {
                try
                {
                    StorageFile file = await folider.GetFileAsync("MyImages.xml");

                    var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    //string text = await Windows.Storage.FileIO.ReadTextAsync(file);
                    StreamReader streamReader = new StreamReader(stream);
                    string text = streamReader.ReadToEnd();
                    streamReader.Dispose();
                    streamReader.Close();
                    return text;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查询收藏夹文件是否存在
        /// </summary>
        /// <returns></returns>
        public async Task<bool> FileExites()
        {
            try
            {
                StorageFile file = await folider.GetFileAsync("MyImages.xml");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<StorageFile> CreateMyImages()
        {
            StorageFile file = await folider.CreateFileAsync("MyImages.xml");
            WallXml xml = new WallXml();
            XmlDocument xmldoc = xml.CreateNewHeader();
            xmldoc.Save(file.Path);
            return file;
        }
    }



    public class WallXmlClass
    {
        public StorageFolder folider = Windows.Storage.ApplicationData.Current.LocalFolder;
    }


    


    public class BigClass
    {
        public string Name { get; set; }
    }
}
