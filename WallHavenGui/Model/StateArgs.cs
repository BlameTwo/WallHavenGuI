using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.WallHavenTools;

namespace WallEventGUI.Model
{
    /// <summary>
    /// 这是关于程序搜索参数的静态资源
    /// </summary>
    public static class StateArgs
    {
        /// <summary>
        /// 颜色分级
        /// </summary>
       public static ObservableCollection<purity>  PublicPurity { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public static ObservableCollection<category> PublicCategories { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public static colors PublicColors { get; set; }
    }
}
