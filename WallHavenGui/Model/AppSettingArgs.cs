using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallEventGUI.Model
{
    public static class AppSettingArgs
    {
        public readonly static string HomeSorting = "HomeSorting";
        public readonly static string HomeOrderBy = "HomeOrderBy";
        public readonly static string HomePurityString = "HomePurityString";
        public readonly static string HomeCategoryString = "HomeCategoryString";
        public readonly static string SearchSave = "SearchSave";
        public readonly static string OpenKey = "OpenKey";
        public readonly static string BaiduFanYiFrom = "BaiduFanYiFrom";
        public readonly static string BaiduFanYiTo = "BaiduFanYiTo";




        public readonly static string SearchPurity = "SearchPurity";
        public readonly static string SearchCat = "SearchCat";
        public readonly static string SearchSorting = "SearchSorting";
        public readonly static string SearchOrderBy = "SearchOrderBy";


        /// <summary>
        /// 为int型，存储用户选择图片清晰挡位
        /// </summary>
        public readonly static string WallPageSize = "WallPageSize";
        /// <summary>
        /// 等待加载完毕
        /// </summary>
        public readonly static string ProStop = "ProStop";
        /// <summary>
        /// 是否新窗口打开
        /// </summary>
        public readonly static string NewWindow = "NewWindow";


        /// <summary>
        /// 收藏一次加载数量
        /// </summary>
        public readonly static string LikePage = "LikePage";

        /// <summary>
        /// 禁忌入口代码
        /// </summary>
        public readonly static string Is18 = "Is18";

        /// <summary>
        /// 是否打开密钥
        /// </summary>
        public readonly static string IsOpenKey = "IsOpenKey";          //入口


        public static bool Exites18(string key)
        {
            if (key == Key18)
            {
                WallHevenSettingResource home = new WallHevenSettingResource();
                home.SettingSetConfig(AppSettingArgs.Is18 ,true.ToString());
                return true;
            }
            return false;
        }



        public readonly static string Key18 = "大撸伤身，小撸怡情";



       static class EncodeAndDecode
        {
            /// <summary>
            /// Base64加密
            /// </summary>
            /// <param name="codeName">加密采用的编码方式</param>
            /// <param name="source">待加密的明文</param>
            /// <returns></returns>
            public static string EncodeBase64(Encoding encode, string source)
            {
                string enstring = "";
                byte[] bytes = encode.GetBytes(source);
                try
                {
                    enstring = Convert.ToBase64String(bytes);
                }
                catch
                {
                    enstring = source;
                }
                return enstring;
            }




            /// <summary>
            /// Base64解密
            /// </summary>
            /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
            /// <param name="result">待解密的密文</param>
            /// <returns>解密后的字符串</returns>
            public static string DecodeBase64(Encoding encode, string result)
            {
                string decode = "";
                byte[] bytes = Convert.FromBase64String(result);
                try
                {
                    decode = encode.GetString(bytes);
                }
                catch
                {
                    decode = result;
                }
                return decode;
            }
        }
    }
}
