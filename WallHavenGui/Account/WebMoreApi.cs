using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.Model;
using WallHavenGui.Account.Model;
using static WallHavenGui.Account.Model.UserModel;

namespace WallHavenGui.Account
{
    public class WebMoreApi
    {
        /// <summary>
        /// 获得用户详情，可以是自己也可以是别人
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<ResultUserModel> GetUserItem(string username)
        {
            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(await GetHtml($"https://wallhaven.cc/user/{username}"));
            ResultUserModel model = new ResultUserModel();
            model.UserName = username;
            var leftnode = htmldoc.DocumentNode.SelectSingleNode("//section[@id='profile-content']//div[@class='user-info-left']");
            var artical = leftnode.SelectSingleNode("./div[1]//article");
            model.UserImage = htmldoc.DocumentNode.SelectSingleNode("//main[@id='main']//header/div/a/img").Attributes["src"].Value;

            #region 拿到个性标签
            //拿到个性标签
            if (artical != null) 
            {
                model.UserTip = leftnode.SelectSingleNode("./div[1]/article/p").InnerText;
            }
            #endregion
            #region 评论
            //拿到评论
            var sections = leftnode.SelectNodes("//section[@id='comments']/article");           //这个评论列表
            
            var section_replies = leftnode.SelectSingleNode("//section[@id='comments']");            //这个是回复列表

            if (sections != null)          
            {
                ObservableCollection< UserComment > comments = new ObservableCollection< UserComment >();
                foreach (var article in sections)
                {
                    string id = article.Attributes["id"].Value;
                    //选择回复列表
                    var comenode = section_replies.SelectNodes($".//div[@id='{id}-replies']//article");
                    var value = GetComment(article);            //主评论
                    if (comenode!= null)
                    {
                        ObservableCollection<UserComment> users = new ObservableCollection<UserComment>();
                        if(comenode!= null || comenode.Count > 0)
                        {
                            foreach (var item in comenode)
                            {
                                users.Add(GetComment(item));
                            }
                            value.comeComment = users;
                        }
                    }
                    comments.Add(value);

                }
                model.Comments = comments;
            }
            #endregion

            #region 详细时间信息
            var noderight = htmldoc.DocumentNode.SelectSingleNode("//section[@id='profile-content']//div[@class='user-info-right']//div[@class='profile-box']");       //
            var first = noderight.SelectSingleNode("./dl[1]");
            var two = noderight.SelectSingleNode("./dl[2]");
            model.LastAction = first.SelectSingleNode("dd[1]/time").InnerText;
            model.GoAction = first.SelectSingleNode("dd[2]/time").InnerText;
            model.UpdataCount = first.SelectSingleNode("dd[3]").InnerText.Trim();
            model.LikeCount = first.SelectSingleNode("dd[4]").InnerText.Trim();
            #endregion

            return model;
        }


        WallHevenSettingResource home = new WallHevenSettingResource();



        public async Task<ResultUserModel> GetLoginItem(string htmlstr)
        {
            return await Task.Run(() =>
            {
                var htmldoc = new HtmlDocument();
                htmldoc.LoadHtml(htmlstr);
                ResultUserModel model = new ResultUserModel();
                model.UserName = home.SettingGetConfig(AppSettingArgs.UserLogin);
                var leftnode = htmldoc.DocumentNode.SelectSingleNode("//section[@id='profile-content']//div[@class='user-info-left']");
                var artical = leftnode.SelectSingleNode("./div[1]//article");
                model.UserImage = htmldoc.DocumentNode.SelectSingleNode("//main[@id='main']//header/div/a/img").Attributes["src"].Value;
                #region 拿到个性标签
                //拿到个性标签
                if (artical != null)
                {
                    model.UserTip = leftnode.SelectSingleNode("./div[1]/article/p").InnerText;
                }
                #endregion
                #region 评论

                //拿到评论
                var sections = leftnode.SelectNodes("//section[@id='comments']/article");           //这个评论列表

                var section_replies = leftnode.SelectSingleNode("//section[@id='comments']");            //这个是回复列表
                int value = 0;
                HtmlNode node = section_replies.SelectSingleNode(".//ul");
               
                if(node != null)                //是否有更多的评论
                {
                    var Max = node.SelectSingleNode("./li[last()]/a").Attributes["href"].Value;         //拿到最后的页数链接
                    value =int.Parse( Max.Substring(Max.LastIndexOf("=") + 1));             //转换成数字，以等号为隔断
                    model.CommentMaxPage = value;
                }

                if (sections != null)
                {
                    ObservableCollection<UserComment> comments = new ObservableCollection<UserComment>();
                    foreach (var article in sections)
                    {
                        string id = article.Attributes["id"].Value;
                        //选择回复列表
                        var comenode = section_replies.SelectNodes($".//div[@id='{id}-replies']//article");
                        var value2 = GetComment(article);            //主评论
                        if (comenode != null)
                        {
                            ObservableCollection<UserComment> users = new ObservableCollection<UserComment>();
                            if (comenode != null || comenode.Count > 0)
                            {
                                foreach (var item in comenode)
                                {
                                    users.Add(GetComment(item));
                                }
                                value2.comeComment = users;
                            }
                        }
                        comments.Add(value2);

                    }
                    model.Comments = comments;
                }
                #endregion
                #region 详细时间信息
                var noderight = htmldoc.DocumentNode.SelectSingleNode("//section[@id='profile-content']//div[@class='user-info-right']//div[@class='profile-box']");       //
                var first = noderight.SelectSingleNode("./dl[1]");
                var two = noderight.SelectSingleNode("./dl[2]");
                model.LastAction = first.SelectSingleNode("dd[1]/time").InnerText;
                model.GoAction = first.SelectSingleNode("dd[2]/time").InnerText;
                model.UpdataCount = first.SelectSingleNode("dd[3]").InnerText.Trim();
                model.LikeCount = first.SelectSingleNode("dd[4]").InnerText.Trim();
                #endregion
                return model;
            });
        }


        public async  Task<string> GetHtml(string url)
        {
            if (AccountArgs.NowCookie == null)
                return null;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);              //创建请求类
            req.CookieContainer = AccountArgs.NowCookie;
            req.Method = "GET";
            req.ContentType = "text/html; charset=UTF-8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36 Edg/99.0.1150.39";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            var request = await req.GetResponseAsync();
            var stream = request.GetResponseStream();
            StreamReader st = new StreamReader(stream);
            return st.ReadToEnd();
        }
        

        /// <summary>
        /// 我的评论
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection< MyCommections>> GetMyCommectionsAsync()
        {
            var myCommections = new ObservableCollection< MyCommections>();
            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(await GetHtml("https://wallhaven.cc/favorites/"));
            var aside = htmldoc.DocumentNode.SelectSingleNode("//aside[@data-storage-id='favorites']");
            var div = aside.SelectSingleNode(".//div");
            var div1 = div.SelectSingleNode("./div[@class='sidebar-content']");
            var ul = div1.SelectSingleNode(".//ul");
            var lis = ul.SelectNodes("./li");
            foreach (var item in lis)
            {
                if (item.Attributes["class"].Value == "trash")          //这个是垃圾桶，不做分类
                    continue;
                MyCommections model = new MyCommections();
                var a = item.SelectSingleNode(".//a[@class='label']");
                model.Name = a.InnerText.Replace(a.SelectSingleNode(".//small").InnerText,"");
                model.ImageCount = a.SelectSingleNode(".//small").InnerText;
                model.Url = item.SelectSingleNode(".//a[@class='label']").Attributes["href"].Value;
                myCommections.Add(model);
            }
            return myCommections;
        }



        /// <summary>
        /// 分离评论
        /// </summary>
        /// <returns></returns>
        public UserComment GetComment(HtmlNode article)
        {
            UserComment comment = new UserComment();
            comment.PicUrl ="https:"+ article.SelectSingleNode("./a/img").Attributes["src"].Value;
            var header = article.SelectSingleNode(".//header");
            comment.UserName = header.SelectSingleNode(".//span/a").InnerText;
            comment.DateTime = header.SelectSingleNode(".//span//time").InnerText;
            comment.Content = article.SelectSingleNode(".//div/p").InnerText;
            return comment;
        }



        public async Task<ObservableCollection< UserComment>> GetMoreComment(int start,int end,ResultUserModel model)
        {
            ResultUserModel resultUserModel = model;
            ObservableCollection<UserComment> returnmodel = new ObservableCollection<UserComment>() ;
            HtmlDocument htmlDocument = new HtmlDocument();
            for (int i = start; i < end + 1; i++)
            {
                if (i > end)
                {
                    break;
                }
                htmlDocument.LoadHtml(await GetHtml("https://wallhaven.cc/user/" + model.UserName + $"?page={i}"));
                var section = htmlDocument.DocumentNode.SelectSingleNode("//section[@id='comments']");
                var node = section.SelectNodes("./article");
                foreach (var item in node)
                {
                    UserComment comment = new UserComment();
                    comment = GetComment(item);
                    comment.comeComment = new ObservableCollection<UserComment>();
                    string id = item.Attributes["id"].Value;
                    var divs = section.SelectNodes($"//div[@id='{id}-replies']/article");  //article
                    if (divs != null)
                    {
                        foreach (var item2 in divs)
                        {
                            comment.comeComment.Add(GetComment(item2));
                        }
                    }
                    returnmodel.Add(comment);
                }
            }
            return returnmodel;
        }





        /// <summary>
        /// 获取用户收藏
        /// </summary>
        /// <param name="url">为跳转链接</param>
        /// <returns></returns>
        public async Task<ObservableCollection<LikeWallpaper>> GetLists(string url)
        {
            try
            {
                HtmlDocument htmldoc = new HtmlDocument();
                ObservableCollection<LikeWallpaper> models = new ObservableCollection<LikeWallpaper>();
                htmldoc.LoadHtml(await GetHtml(url));
                HtmlNode thumbs = htmldoc.DocumentNode.SelectSingleNode("//div[@id='thumbs']");
                HtmlNodeCollection sections = thumbs.SelectNodes(".//section");
                if(sections != null)
                {
                    foreach (HtmlNode section in sections)
                    {
                        HtmlNodeCollection ul = section.SelectNodes("./ul/li");
                        foreach (var item in ul)
                        {
                            LikeWallpaper likeWallpaper = new LikeWallpaper();

                            HtmlNode figure = item.SelectSingleNode(".//figure");
                            likeWallpaper.Id = figure.Attributes["data-wallpaper-id"].Value;
                            if(figure.SelectSingleNode(".//img") != null)
                                likeWallpaper.PicUrl = figure.SelectSingleNode(".//img").Attributes["data-src"].Value;
                            else
                            {
                                likeWallpaper.PicUrl = "";
                            }
                            var div = figure.SelectSingleNode(".//div[@class='thumb-info']");
                            likeWallpaper.Like = div.SelectSingleNode(".//a").InnerText;
                            likeWallpaper.Size = div.SelectSingleNode(".//span").InnerText;
                            models.Add(likeWallpaper);
                        }
                    }

                }
                return models;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
