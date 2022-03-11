using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WallEventGUI.WallHavenTools;

namespace WallHavenGui.Model
{

    /// <summary>
    /// 有道翻译，不稳定，已经弃用，正式版会清理掉。
    /// </summary>
    public class YouDaoFanYi
    {
        private async Task<string> HttpGet(string api)
        {
            string serviceAddress = api;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
            request.Method = "GET";
            WebResponse response = await request.GetResponseAsync();
            Stream myResponseStream =  response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream);
            //返回json字符串
            return myStreamReader.ReadToEnd();
        }


        public async Task<ObservableCollection<YouDaoModel>> FanYi(string type,string content)
        {
            JObject json = JObject.Parse(await HttpGet($@"http://fanyi.youdao.com/translate?&doctype=json&type={type}&i={content}"));
            ObservableCollection<YouDaoModel> youDaoModels = new ObservableCollection<YouDaoModel>();
            
            string Jo = json["translateResult"].ToString();

            string utf8_string = Encoding.UTF8.GetString(Encoding.Default.GetBytes(Jo));
            JArray ja2 = JArray.Parse(utf8_string);
            foreach (var item in ja2)
            {
                JArray jArray = JArray.Parse(item.ToString());
                foreach (var item2 in jArray)
                {
                    YouDaoModel model = new YouDaoModel()
                    {
                        src = item2["src"].ToString(),
                        tgt = item2["tgt"].ToString()
                    };
                    youDaoModels.Add(model);
                }
            }
            return youDaoModels;
        }


        public class YouDaoModel
        {
            public string src { get; set; }
            public string tgt { get; set; }
        }
    }

    public class FanYi
    {

        public static async Task<Rootobject> Baidu_Translate(string from, string to, string content)
        {
            // 原文
            string q = content;
            // 源语言
            // 改成您的APP ID
            string appId = "20210514000827165";
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            // 改成您的密钥
            string secretKey = "VLZcrCyLan2T6qIuza80";
            string sign = EncryptString(appId + q + salt + secretKey);
            string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
            url += "q=" + HttpUtility.UrlEncode(q);
            url += "&from=" + from;
            url += "&to=" + to;
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = 6000;
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();


            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream myResponseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        Debug.WriteLine(retString);
                        var result = JsonConvert.DeserializeObject<Rootobject>(retString);
                        return result;
                    }
                }


            }






        }

        // 计算MD5值
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }


        public class Rootobject
        {
            public string from { get; set; }
            public string to { get; set; }
            public string domain { get; set; }
            public int type { get; set; }
            public int status { get; set; }

            public int error { get; set; }
            public string msg { get; set; }

            public Trans_Result[] trans_result { get; set; }
        }

        public class Trans_Result
        {
            public string src { get; set; }
            public string dst { get; set; }

            public int prefixWrap { get; set; }

            public object[] relation { get; set; }
            public object[][] result { get; set; }
        }
    }



}
