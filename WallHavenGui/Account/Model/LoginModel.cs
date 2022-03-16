using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallHavenGui.Account.Model
{
    public class LoginModel
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ResultLoginModel
    {
        public string UserName { get; set; }

        public string UserKey { get; set; }
    }
}
