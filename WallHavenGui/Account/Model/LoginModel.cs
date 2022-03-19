using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WallHavenGui.Account.Model
{
    public static class AccountArgs
    {
        public static CookieContainer NowCookie { get; set; }
    }

    public class Account
    {

        /// <summary>
        /// 退出登录，请务必等待此方法
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Loginout()
        {
            await Task.Run(async () =>
            {
                if (AccountArgs.NowCookie == null)
                {
                    throw new NotImplementedException("无账号信息");
                }
                else
                {
                    var result = await GetPost(Model.GetType.OutLogin, null);
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    //ZYFfsdfa
                    parameters.Add("_token", result.Token);
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));   //使用utf-8格式组装post参数
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://wallhaven.cc/auth/logout");              //创建请求类
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36 Edg/99.0.1150.39";
                    HttpWebResponse respinse = (HttpWebResponse)req.GetResponse();
                    Stream st = respinse.GetResponseStream();
                    StreamReader reader = new StreamReader(st);
                    AccountArgs.NowCookie = null;
                }
            });
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<ResultLoginModel> Login(string username, string password)
        {
            var resunlt = await Task.Run(async () =>
            {
                ResultLoginModel model = new ResultLoginModel();
                PostModel args = await GetPost(Model.GetType.Login, null);
                #region 进行登录
                CookieContainer cc = GetDIct(args.Cookie);                //使用方法拼接cookie

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                //ZYFfsdfa
                parameters.Add("username", username);         //用户账号
                parameters.Add("password", password);            //用户密码
                parameters.Add("_token", args.Token);                //用户登录凭证
                byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));   //使用utf-8格式组装post参数
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://wallhaven.cc/auth/login");              //创建请求类
                req.Method = "POST";            //方式为POST
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36 Edg/99.0.1150.39";
                req.CookieContainer = cc;           //设置cookie
                req.ContentLength = postData.Length;            //设置发送字节数限制
                req.Referer = "https://wallhaven.cc/login";             //设置http标头，这个可以在浏览器的开发人员工具中看到

                Stream reqStream = req.GetRequestStream();              //获得stream流
                reqStream.Write(postData, 0, postData.Length);          //写入字节，也就是自己的POST内容
                WebResponse wr = req.GetResponse();         //获得请求成功后的页面，也就是个人信息页面，现在还只是资源，需要下一步流化
                reqStream.Close();          //关闭流
                Stream respStream = wr.GetResponseStream();           //转化为stream流

                var coo = wr.Headers.GetValues("Set-Cookie").ToList();              //获得登陆后cookie，这和登录前cookie不一样的！并且登陆后cookie可以干很多事情！
                AccountArgs.NowCookie = GetDIct(coo);
                System.IO.StreamReader reader = new System.IO.StreamReader(respStream, System.Text.Encoding.GetEncoding("utf-8"));          //编码序列
                string t = reader.ReadToEnd();          //获得登陆后得节过页面，然后进行判断

                HtmlDocument htmldoc = new HtmlDocument();
                htmldoc.LoadHtml(t);

                var node5 = htmldoc.DocumentNode.SelectSingleNode("//main//div[@id='notifications']/p").InnerText;
                if (node5 == "Your username/password combination was incorrect")         //判断登录是否失败
                {
                    return null;
                }
                var userName = htmldoc.DocumentNode.SelectSingleNode("//div[@id='user']/h1/a").InnerText;
                model.UserName = userName;
                #endregion


                #region 获取密钥
                //https://wallhaven.cc/settings/account
                HttpWebRequest http2 = (HttpWebRequest)HttpWebRequest.Create("https://wallhaven.cc/settings/account");          //发起请求
                http2.Method = "GET";               //GET方式
                http2.ContentType = "application/x-www-form-urlencoded";                //请求内容，其实无所谓，可删，包含下面的UserAgent
                http2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36 Edg/99.0.1150.39";
                http2.CookieContainer = GetDIct(coo);           //设置之前登陆后的Cookie
                var res2 = (HttpWebResponse)http2.GetResponse();                //获得请求页面的资源
                Stream stream = res2.GetResponseStream();              //stream化
                StreamReader reader2 = new StreamReader(stream);            //开始读取
                string txt2 = reader2.ReadToEnd();          //读取完毕，txt2中存储的就是获得后的页面！可以使用html解析器解析页面
                #endregion
                htmldoc.LoadHtml(txt2);
                var node3 = htmldoc.DocumentNode.SelectSingleNode
                    ("//form[@action='https://wallhaven.cc/settings/account']/p[3]//input[@type='text' and @readonly='readonly']").Attributes["value"].Value;
                model.UserKey = node3;
                var node4 = htmldoc.DocumentNode.SelectSingleNode("//input[@name='email']").Attributes["value"].Value;
                model.emaile = node4;
                wr.Close();         //释放页面！
                return model;
            });
            return resunlt;
        }







        public async Task<PostModel> GetPost(GetType type, string wallid)
        {
            return await Task.Run(() =>
            {
                PostModel login = new PostModel();
                HttpWebRequest request = null;
                switch (type)
                {
                    case Model.GetType.Login:
                        request = (HttpWebRequest)WebRequest.Create("https://wallhaven.cc/login");               //先去登录页面获得临时登录凭证，也就是POST中获得的_tokan
                        break;
                    case Model.GetType.OutLogin:
                        request = (HttpWebRequest)WebRequest.Create("https://wallhaven.cc");
                        break;
                    case Model.GetType.AddMyLike:
                        request = (HttpWebRequest)WebRequest.Create($"https://wallhaven.cc/w/{wallid}");
                        break;
                }
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                var response = (HttpWebResponse)request.GetResponse();
                var resultAsync = request.GetResponseAsync();
                var a = response.Headers.GetValues("Set-Cookie").ToList();                //获得cookie
                var xml = new HtmlDocument();
                xml.Load(response.GetResponseStream());             //直接使用HTML分析器分析html文档
                var xmlel = xml.DocumentNode.SelectSingleNode("//meta[@name='csrf-token']");           //定位到凭证节点
                var tokan = xmlel.Attributes["content"].Value;                //获得凭证
                login.Token = tokan;
                login.Cookie = a;
                return login;
            });
        }



        private static CookieContainer GetDIct(List<string> a)
        {
            CookieContainer cc = new CookieContainer();
            foreach (var item in a)
            {
                Cookie cookie = new Cookie();
                cookie.Name = item.Substring(0, item.IndexOf('='));
                string fenhao = item.Substring(item.IndexOf('=') + 1).Replace(",", "%2c");
                cookie.Value = fenhao.Substring(0, fenhao.IndexOf(";"));
                //cookie.Path = "/";
                cookie.Domain = "wallhaven.cc";
                cc.Add(cookie);
            }
            return cc;
        }


        /// <summary>
        /// 网上查的资料，后面学习
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        private static string BuildQuery(IDictionary<string, string> parameters, string encode)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }
                    postData.Append(name);
                    postData.Append("=");
                    if (encode == "gb2312")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                    }
                    else if (encode == "utf8")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    }
                    else
                    {
                        postData.Append(value);
                    }
                    hasParam = true;
                }
            }
            return postData.ToString();
        }
    }

    public enum GetType
    {
        Login,
        OutLogin,
        AddMyLike
    }


    public class PostModel
    {
        public string Token { get; set; }
        public List<string> Cookie { get; set; }
    }

    public class ResultLoginModel
    {
        public string UserName { get; set; }

        public string UserKey { get; set; }

        public string emaile { get; set; }
    }


    public class ResultUserModel
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 上次活动时间
        /// </summary>
        public string LastAction { get; set; }

        /// <summary>
        /// 加入时间！
        /// </summary>
        public string GoAction { get; set; }

        /// <summary>
        /// 上传数量
        /// </summary>
        public string UpdataCount { get; set; }

        /// <summary>
        /// 收藏夹数量
        /// </summary>
        public string LikeCount { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        public string UserTip { get; set; }

        /// <summary>
        /// 别人给你的评论
        /// </summary>
        public ObservableCollection<UserComment> Comments { get; set; }
    }




    public class UserComment
    {
        /// <summary>
        /// 用户签名
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string PicUrl { get; set; }


        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 回信列表
        /// </summary>
        public ObservableCollection<UserComment> comeComment { get; set; }

        
    }

}
