using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace WallEventGUI.WallHavenTools
{
    public class Args
    {
        public string Key { get; set; }

        public JObject Setting { get; set; }
    }


    public class WebGet : Args
    {

        public WebGet()
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 1000000000;
            
        }
        /// <summary>
        /// 不公开方法同步获取
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetJson(string url)
        {
            System.GC.Collect();
            Thread.Sleep(250);
            string Url;
            if (Key != null)
                Url = url + $"&apikey={Key}";
            Url = url;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.Proxy = null;
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentType = "application/json;charset=utf-8";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string result= reader.ReadToEnd();
            response.Close();
            response.Dispose();
            return result;
            
        }




        /// <summary>
        /// 可以直接用此方法获得Json，支持异步
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetJsonAsync(string url)
        {
            return await Task.Run(() => GetJson(url));
        }
    }

    /// <summary>
    /// 此类继承KEY
    /// </summary>
    public class Tools : Args
    {
        public Tools()
        {
            webget = new WebGet();
            OpenKey = false;
        }
        WebGet webget;
        public bool OpenKey { get; set; }
        /// <summary>
        /// 通过apikey获得用户设置
        /// </summary>
        /// <returns></returns>
        public async Task<SettingArg> GetUserConfigAsync()
        {
            var result = await Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(Key))
                {
                    SettingArg arg = new SettingArg();
                    string key = OpenKey ? $"apikey={Key}" : " ";
                    var j = await webget.GetJsonAsync($"https://wallhaven.cc/api/v1/settings?{key}");
                    if (j == null) return null;
                    JObject jobject = JObject.Parse(j);
                    arg.PageCount = Convert.ToInt32(jobject["data"]["per_page"].ToString());
                    arg.toplist_range = jobject["data"]["toplist_range"].ToString();
                    return arg;
                }
                return null;
            });
            if (result != null)
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 异步方式
        /// </summary>
        /// <param name="id">壁纸id</param>
        /// <returns></returns>
        public async Task<Wallpaper> GetWallpaperAsync(string id)
        {
            var result = await Task.Run(async () =>
            {
                string key = OpenKey ? $"{Key}" : " ";
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string json = await webget.GetJsonAsync($"https://wallhaven.cc/api/v1/w/{id}?apikey={key}");
                JObject jo = JObject.Parse(json);
                if (json == null||jo["error"] != null)            //不为空的话表示没有错误信息,或者请求内容返回null
                {
                    return null;
                }
                Wallpaper wall = new Wallpaper()
                {
                    url = jo["data"]["url"].ToString(),
                    id = jo["data"]["id"].ToString(),
                    Views = jo["data"]["views"].ToString(),
                    LikeCount = jo["data"]["favorites"].ToString(),
                    Height = int.Parse(jo["data"]["dimension_y"].ToString()),
                    Width = int.Parse(jo["data"]["dimension_x"].ToString()),
                    resolution = jo["data"]["resolution"].ToString(),
                    FileSize = long.Parse(jo["data"]["file_size"].ToString()),
                    ImageType = jo["data"]["file_type"].ToString(),
                    CreateTime = DateTime.Parse(jo["data"]["created_at"].ToString()),
                    WallpaperUrl = jo["data"]["path"].ToString(),
                };
                switch (jo["data"]["purity"].ToString().ToUpper())
                {
                    case "NSFW": { wall.Purity = purity.NSFW; break; }
                    case "SKF": { wall.Purity = purity.SFW; break; }
                    case "SKETCHY": { wall.Purity = purity.Sketchy; break; }
                }
                switch (jo["data"]["category"].ToString().ToUpper())
                {
                    case "GENERAL": { wall.Category = category.General; break; }
                    case "ANIME": { wall.Category = category.Anime; break; }
                    case "POPELE": { wall.Category = category.People; break; }
                }
                

                wall.colors = GetColor(JArray.Parse(jo["data"]["colors"].ToString()));

                var tags = new ObservableCollection<WallpaperTag>();
                JArray tagja2 = JArray.Parse(jo["data"]["tags"].ToString());
                foreach (var item2 in tagja2)
                {
                    WallpaperTag tag = new WallpaperTag()
                    {
                        TagId = item2["id"].ToString(),
                        alias = item2["alias"].ToString(),
                        TagName = item2["name"].ToString(),
                        Category_Id = item2["category_id"].ToString(),
                        Category_Name = item2["category"].ToString(),
                        CreateTimeTag = DateTime.Parse(item2["created_at"].ToString()),
                    };
                    tags.Add(tag);
                }
                wall.favorites = jo["data"]["favorites"].ToString();
                wall.Thumbs = GetThumbs(JObject.Parse(jo["data"]["thumbs"].ToString()));
                wall.User = GetWallpaperUser(JObject.Parse(jo["data"]["uploader"].ToString()));
                wall.Tags = tags;

                return wall;
            });
            if (result != null)
            {
                return result;
            }
            return null;
        }


        WallpaperUser GetWallpaperUser(JObject jo)
        {
            WallpaperUser user = new WallpaperUser()
            {
                UserName = jo["username"].ToString(),
                Group= jo["group"].ToString(),
                Images = new UserImage()
            };
            var js = JSONDictionary.Parse(jo["avatar"].ToString());
            string a, b, c, d;
            js.TryGetValue("20px", out a);js.TryGetValue("32px", out b);js.TryGetValue("128px", out c);js.TryGetValue("200px", out d);
            user.Images.W20px = a;user.Images.W32px = b;user.Images.W128px = c;user.Images.W200px = d;
            return user;
        }


        /// <summary>
        /// 创建新的Dictionary对象
        /// 从Dictionary<string, int>对象继承
        /// </summary>
        [Serializable]
        public class JSONDictionary : Dictionary<string, string>
        {
            /// <summary>
            /// 覆盖ToString方法
            /// 实现将C#对象序列化为JSON格式
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("{");
                int i = 0;
                foreach (KeyValuePair<string, string> item in this)
                {
                    if (i > 0)
                        sb.Append(",");
                    sb.AppendFormat("\"{0}\":{1}", item.Key, item.Value);
                    i++;
                }
                sb.Append("}");
                return sb.ToString();
            }

            /// <summary>
            /// 为JSONDictionary对象添加静态方法
            /// 实现从JSON格式反序列化为C#对象
            /// </summary>
            /// <param name="jsonString">符合要求的JSON格式</param>
            /// <returns></returns>
            public static JSONDictionary Parse(string jsonString)
            {
                JSONDictionary jsonDictionary = new JSONDictionary();
                try
                {
                    string[] items = jsonString.Replace("{", string.Empty)
                                         .Replace("}", string.Empty)
                                         .Split(',');
                    foreach (string item in items)
                    {
                        string keyValue = item.Substring(item.IndexOf(':')+1).Replace("\r\n","").Trim().Trim('"');
                        string keyName = item.Substring(0, item.IndexOf(':')).Replace("\r\n", "").Trim().Trim('"');
                        jsonDictionary.Add(keyName, keyValue);
                    }

                    return jsonDictionary;
                }
                catch
                {
                    throw new Exception("对象转换失败");
                }
            }
        }




        /// <summary>
        /// 综合搜索
        /// </summary>
        /// <param name="TopRange">事件匹配</param>
        /// <param name="Sorting">搜索类型</param>
        /// <param name="keyword">关键字</param>
        /// <param name="Categorys">分类</param>
        /// <param name="Purities">颜色等级，需要配置<see cref="Args.Key"/></param>
        /// <param name="Order">排序方式</param>
        /// <param name="Pagesize">页数</param>
        /// <param name="Ratios">比例</param>
        /// <returns></returns>
        //public async Task<Wallpapers> GetSearchWallpaper(
        //    sorting Sorting,
        //    topRange TopRange,
        //    string keyword,
        //    ObservableCollection<ratios> Ratios,
        //    ObservableCollection<category> Categorys
        //    , ObservableCollection<purity> Purities,
        //    order Order,
        //    int Pagesize,
        //    colors Colors)
        //{
        //    string range = GetRange(TopRange);
        //    switch (Sorting)
        //    {
        //        case sorting.toplist:
        //            if (TopRange != null) return await GetTopListAsync(keyword, GetPurity(Purities),
        //                GetCategory(Categorys), range, Order, Pagesize,
        //                GetRatios(Ratios), GetEnumColor(Colors));
        //            break;
        //    }
        //    return await GetSearchDefult(keyword, GetCategory(Categorys),
        //               GetPurity(Purities), GetRatios(Ratios),
        //               GetEnumColor(Colors), Order, Pagesize, Sorting); ;
        //}

        /// <summary>
        /// 字符串搜索方法
        /// </summary>
        /// <param name="TopRange">事件匹配</param>
        /// <param name="Sorting">搜索类型</param>
        /// <param name="keyword">关键字</param>
        /// <param name="Categorys">分类</param>
        /// <param name="Purities">颜色等级，需要配置<see cref="Args.Key"/></param>
        /// <param name="Order">排序方式</param>
        /// <param name="Pagesize">页数</param>
        /// <param name="Ratios">比例</param>
        /// <returns></returns>
        public async Task<Wallpapers> GetSearchWallpaperString(
            sorting Sorting,
            topRange TopRange,
            string keyword,
            string Ratios,
            string Categorys
            , string Purities,
            order Order,
            int Pagesize,
            colors Colors)
        {
            string range = GetRange(TopRange);
            switch (Sorting)
            {
                case sorting.toplist:
                    if (TopRange != null) return await GetTopListAsync(keyword, Purities,
                        Categorys, range, Order, Pagesize,
                        Ratios, GetEnumColor(Colors));
                    break;
            }
            return await GetSearchDefult(keyword,Categorys,
                       Purities, Ratios,
                       GetEnumColor(Colors), Order, Pagesize, Sorting); ;
        }


        public string GetRatios(ObservableCollection<ratios> Ratios)
        {
            string str = "";
            if (Ratios[0] == ratios.None)
                return "";
            foreach (var item in Ratios)
            {

                str += $"{item.ToString().Substring(item.ToString().LastIndexOf('S') + 1)},";
            }
            return str;
        }
        async Task<Wallpapers> GetSearchDefult(string keyword, string Categories, string Puritys,
            string Ratios, string colors, order Order, int Pagesize, sorting Sorting)
        {
            return await Task.Run(async () => {
                string key = OpenKey ? $"apikey={Key}" : " ";
                string url = $"https://wallhaven.cc/api/v1/search?{key}&q={keyword}&" +
                $"categories={Categories}" +
                $"&purity={Puritys}" +
                $"&order={Order.ToString()}" +
                $"&colors={colors}" +
                $"&ratios={System.Web.HttpUtility.UrlEncode(Ratios)}" +
                $"&sorting={Sorting}&page={Pagesize}";
                JObject a = JObject.Parse(await webget.GetJsonAsync(url));
                Wallpapers walls = new Wallpapers();
                walls.WallpaperList = GetWallpaper(JArray.Parse(a["data"].ToString()));
                walls.Meta = GetMeta(a);
                return walls;
            });
        }





        public async Task<Wallpapers> GetTopListAsync(string keyword, string Purity, string Categorie,
            string toprange, order ord, int pagesize, string Ratios, string colors)           //GetToplist的方法有些特殊，必须要使用上时间筛选才可，默认情况下时间筛选为一个月。
        {
            //Task.Run()异步一个新线程搞一个委托，去获得数据
            return await Task.Run(async () =>
            {
                string key = OpenKey ? $"apikey={Key}" : " ";
                string day = toprange.ToString();
                string url = $"https://wallhaven.cc/api/v1/search?{key}&q={keyword}&sorting=toplist&categories={Categorie}&purity={Purity}&topRange={day}&order={ord.ToString()}&colors={colors}&ratios={System.Web.HttpUtility.UrlEncode(Ratios)}&page={pagesize}";
                JObject a = JObject.Parse(await webget.GetJsonAsync(url));
                Wallpapers walls = new Wallpapers();
                walls.WallpaperList = GetWallpaper(JArray.Parse(a["data"].ToString()));
                walls.Meta = GetMeta(a);
                return walls;
            });
        }


        public Meta GetMeta(JObject jo)
        {
            Meta meta = new Meta();
            meta.Last_Page = jo["meta"]["last_page"].ToString();
            meta.Quert = jo["meta"]["query"].ToString();
            meta.ToTal = jo["meta"]["total"].ToString();
            meta.Seed = jo["meta"]["seed"].ToString();
            return meta;
        }




        public ObservableCollection<Wallpaper> GetWallpaper(JArray a)
        {
            ObservableCollection<Wallpaper> walls = new ObservableCollection<Wallpaper>();
            foreach (var item in a)
            {
                Wallpaper wall = new Wallpaper();
                wall.id = item["id"].ToString();
                wall.url = item["url"].ToString();
                wall.Views = item["views"].ToString();
                wall.LikeCount = item["favorites"].ToString();
                wall.source = item["source"].ToString();
                switch (item["purity"].ToString().ToUpper())           //这里做了照搬，后面优化
                {
                    case "NSFW": { wall.Purity = purity.NSFW; break; }
                    case "SKF": { wall.Purity = purity.SFW; break; }
                    case "SKETCHY": { wall.Purity = purity.Sketchy; break; }
                }
                switch (item["category"].ToString().ToUpper())
                {
                    case "GENERAL": { wall.Category = category.General; break; }
                    case "ANIME": { wall.Category = category.Anime; break; }
                    case "POPELE": { wall.Category = category.People; break; }
                }
                wall.Height = int.Parse(item["dimension_y"].ToString());
                wall.Width = int.Parse(item["dimension_x"].ToString());
                wall.FileSize = long.Parse(item["file_size"].ToString());
                
                wall.ImageType = item["file_type"].ToString();
                wall.CreateTime = DateTime.Parse(item["created_at"].ToString());
                wall.resolution = item["resolution"].ToString();
                wall.colors = GetColor(JArray.Parse(item["colors"].ToString()));
                wall.WallpaperUrl = item["path"].ToString();
                wall.favorites = item["favorites"].ToString();
                wall.Thumbs = GetThumbs(JObject.Parse(item["thumbs"].ToString()));
                walls.Add(wall);
            }
            return walls;
        }




        thumbs GetThumbs(JObject jo)
        {
            thumbs th = new thumbs();
            th.large = jo["large"].ToString();
            th.original = jo["original"].ToString();
            th.small = jo["small"].ToString();
            return th;
        }

        ObservableCollection<ColorTag> GetColor(JArray ja)
        {
            ObservableCollection<ColorTag> color = new ObservableCollection<ColorTag>();
            foreach (var item in ja)
            {
                ColorTag tag = new ColorTag()
                {
                    color = item.ToString(),
                    name =item.ToString()
                };
                color.Add(tag);
            }
            return color;
        }

        public string GetEnumColor(colors Colors)
        {
            if (Colors == colors.None) return "";
            if (Colors.ToString().Length > 6 ? true : false)
            {
                return Colors.ToString().Substring(Colors.ToString().IndexOf('C') + 1);
            }
            return Colors.ToString();
        }

        string GetRange(topRange top)
        {
            switch (top)
            {
                case topRange.D1: return "1d";
                case topRange.D3: return "3d";
                case topRange.W1: return "1w";
                case topRange.M1: return "1M";
                case topRange.M3: return "3M";
                case topRange.M6: return "6M";
                case topRange.Y1: return "1Y";
                default: return "1M";
            }
        }




        string GetCategory(ObservableCollection<category> categories)
        {
            bool one = false, two = false, there = false;
            for (int i = 0; i < categories.Count; i++)
            {
                switch (categories[i])
                {
                    case category.General: one = true; break;
                    case category.Anime: two = true; break;
                    case category.People: there = true; break;
                }
            }
            return EnumGet(one, two, there);
        }

        string EnumGet(bool one, bool two, bool there)
        {
            string a = one ? "1" : "0";
            string b = two ? "1" : "0";
            string c = there ? "1" : "0";
            return a + b + c;
        }

        string GetPurity(ObservableCollection<purity> purities)     //处理枚举，因为两个枚举基本类似，就采用函数。
        {
            bool one = false, two = false, there = false;
            for (int i = 0; i < purities.Count; i++)
            {
                switch (purities[i])
                {
                    case purity.SFW: one = true; break;
                    case purity.Sketchy: two = true; break;
                    case purity.NSFW: there = true; break;
                }
            }
            return EnumGet(one, two, there);
        }
    }
}
