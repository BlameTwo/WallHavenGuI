using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallHavenGui.Account;
using WallHavenGui.Account.Model;

namespace WallHavenGui.ViewModel
{
    public class CommentVM: ObservableObject
    {
        public CommentVM()
        {
            MoreData = new AsyncRelayCommand(moredata);
            nowpage++;
        }
        int nowpage = 1;
        private async Task moredata()
        {
            WebMoreApi api = new WebMoreApi();
            int count = MyComment.Comments.Count;
            var value = await api.GetMoreComment(nowpage,MyComment.CommentMaxPage,MyComment);
            if(value.Count != count)
            {
                foreach (var item in value)
                {
                    MyComment.Comments.Add(item);
                }
                nowpage++;
            }
            else
            {
                InfoBar info = new InfoBar();
                info.Title = "已经拉取所有评论";
                info.Message = "评论已经全部拉去，如果未被拉取请反馈BUG";
                info.IsOpen = true;
            }

        }

        private ResultUserModel _MyComment;

        public ResultUserModel MyComment
        {
            get { return _MyComment; }
            set { _MyComment = value; OnPropertyChanged(); }
        }

        public AsyncRelayCommand MoreData { get; set; }


    }
}
