using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WallHavenGui.Account.Model;

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
            if (AccountArgs.NowCookie == null)
                return null;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"https://wallhaven.cc/user/{username}");              //创建请求类
            req.CookieContainer = AccountArgs.NowCookie;
            req.Method = "GET";
            req.ContentType = "text/html; charset=UTF-8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36 Edg/99.0.1150.39";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            var request = await req.GetResponseAsync();
            var stream = request.GetResponseStream();
            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.Load(stream);
            ResultUserModel model = new ResultUserModel();
            model.UserName = username;
            var leftnode = htmldoc.DocumentNode.SelectSingleNode("//section[@id='profile-content']//div[@class='user-info-left']");
            var artical = leftnode.SelectSingleNode("./div[1]//article");
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

            if (sections != null || sections.Count>0)          
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



        /// <summary>
        /// 分离评论
        /// </summary>
        /// <returns></returns>
        public UserComment GetComment(HtmlNode article)
        {
            UserComment comment = new UserComment();
            comment.PicUrl = article.SelectSingleNode("./a/img").Attributes["src"].Value;
            var header = article.SelectSingleNode(".//header");
            comment.UserName = header.SelectSingleNode(".//span/a").InnerText;
            comment.DateTime = header.SelectSingleNode(".//span//time").InnerText;
            comment.Content = article.SelectSingleNode(".//div/p").InnerText;
            return comment;
        }
    }
}
