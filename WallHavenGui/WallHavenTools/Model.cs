using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace WallEventGUI.WallHavenTools
{
    
    public class More
    {
        public string source { get; set; }

        public thumbs Thumbs { get; set; }
    }

    public interface IImage
    {
        /// <summary>
        /// 图片加载动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ImageOpend(object sender, RoutedEventArgs e);
    }

      
    public class Wallpapers
    {
        public ObservableCollection<Wallpaper> WallpaperList { get; set; }
        public Meta Meta { get; set; }
    }

    public class SettingArg
    {
        /// <summary>
        /// 用户一页数据量
        /// </summary>
        public int PageCount { get; set; }

        public string toplist_range { get; set; }
    }



    public class Wallpaper : More, IImage
    {
        public string id { get; set; }
        public string url { get; set; }
        public string favorites { get; set; }

        public WallpaperUser User { get; set; }

        public string Views { get; set; }

        public string LikeCount { get; set; }

        public purity Purity { get; set; }

        public category Category { get; set; }

        public int Height { get; set; }     //dimension_y为高度，Y轴

        public int Width { get; set; }      //dimension_x为宽度,x轴

        public string resolution { get; set; }      //分辨率

        public long FileSize { get; set; }

        public string ImageType { get; set; }

        public ObservableCollection<WallpaperTag> Tags { get; set; }

        public DateTime CreateTime { get; set; }

        public ObservableCollection<ColorTag> colors { get; set; }

        public string WallpaperUrl { get; set; }

        public void ImageOpend(object sender, RoutedEventArgs e)
        {
            ////这个方法会让数据项加载出现闪动，暂时不修改
            //var image = sender as Image;
            //DoubleAnimation da = new DoubleAnimation()
            //{
            //    From = 0,
            //    To = 1,
            //    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 600))
            //};

            //Storyboard.SetTarget(da, image);
            //Storyboard.SetTargetProperty(da, "Image.Opacity");
            //Storyboard sb = new Storyboard();
            //sb.Children.Add(da);
            //sb.Begin();
        }
    }


    public class ColorTag
    {
        public string color { get; set; }
        public string name { get; set; }
    }


    public class WallpaperUser
    {
        public string UserName { get; set; }
        public string Group { get; set; }
        public UserImage Images { get; set; }
    }


    public class Meta
    {
        public string Current_Page { get; set; }
        public string Last_Page { get; set; }
        public string Quert { get; set; }
        public string ToTal { get; set; }
        public string Seed { get; set; }
    }

    public class thumbs
    {
        public string large { get; set; }
        public string original { get; set; }
        public string small { get; set; }
    }

    public class WallpaperTag
    {
        public string TagId { get; set; }
        public string TagName { get; set; }

        public string alias { get; set; }

        public string Category_Id { get; set; }

        public string Category_Name { get; set; }

        public DateTime CreateTimeTag { get; set; }

    }

    public class UserImage
    {
        public string W200px { get; set; }
        public string W128px { get; set; }
        public string W32px { get; set; }
        public string W20px { get; set; }
    }

    public enum purity
    {
        SFW, Sketchy, NSFW
    }

    public enum category
    {
        General, Anime, People
    }

    /// <summary>
    /// D1为一天，D3为三天，W1为一周，M1为一月，M3为三个月，M6为半年，Y1为一年
    /// </summary>
    public enum topRange
    {
        D1, D3, W1, M1, M3, M6, Y1
    }

    public enum ratios
    {
        WS19X9, WS16X10,
        WSS21X9, WSS32X9, WSS48X9,
        WSSS9X16, WSSS10X16, WSSS9X18,
        WSSSS1X1, WSSSS3X2, WSSSS4X3, WSSSS5X4,
        None
    }


    public enum colors
    {
        C660000, C990000, cc0000, cc3333, ea4c88,
        C993399, C663399, C333399, C0066CC, C0099CC,
        C66cccc, C77cc33, C669900, C336600, C666600,
        C999900, CCCC33, FFFF00, FFcc33, FF9900,
        FF6600, CC6633, C996633, C663300, C000000,
        C999999, CCCCCC, FFFFFF, C424153,
        None
    }


    public static class GetModel
    {
        public static Dictionary<string, string> GetRatio<T>(char flage)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (int mycode in Enum.GetValues(typeof(T)))
            {
                string strName = Enum.GetName(typeof(T), mycode);
                dict.Add(strName, strName.Substring(strName.LastIndexOf(flage) + 1));
            }
            return dict;
        }
    }

    /// <summary>
    /// data_added为添加日期，relevance为相关性,random为随机，views为观看次数，favorites为收藏数量，toplist为查找热门
    /// </summary>
    public enum sorting
    {
        date_added, relevance, random, views, favorites, toplist,hot
    }

    public enum order
    {
        desc, asc
    }

    /// <summary>
    /// 标签详细信息，获取标签之时，常规，动漫，人物三个分类是都选上的，只有颜色分级
    /// </summary>
    public class Tag
    {
        public string Id { get; set; }

        public string name { get; set; }
        public string alias { get; set; }

        /// <summary>
        /// 上一级标签id
        /// </summary>
        public string Category_Id { get; set; }

        /// <summary>
        /// 上一级标签名
        /// </summary>
        public string Category { get; set; }
        public purity Purity { get; set; }
        public DateTime Created { get; set; }
    }

        


    /// <summary>
    /// https://wallhaven.cc/api/v1/collections?apikey=??       https://wallhaven.cc/api/v1/collections/用户名，必须是用户公开集合……挺无语的
    /// </summary>
    public class UserList
    {
        public string Id { get; set; }
        public string label { get; set; }
        public string views { get; set; }
        public string _public { get; set; }
        public string count { get; set; }
    }



    
}
