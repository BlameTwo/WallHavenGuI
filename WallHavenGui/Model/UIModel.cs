using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.WallHavenTools;

namespace WallEventGUI.Model
{
    public class GitClass
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string By { get; set; }
    }


    public class UIModel
    {
        public class LevelCs
        {
            public bool Level1 { get; set; }
            public bool Level2 { get; set; }
            public bool Level3 { get; set; }

        }

        

        public class SearchArgs
        {
            public string KeyWord { get; set; }
            public string PurityString { get; set; }

            public bool NewWindow { get; set; }
        }

        
        public class Sorting
        {
            public bool Data_Added { get; set; }
            public bool Relevance { get; set; }
            public bool Random { get; set; }
            public bool Views { get; set; }
            public bool Favorites { get; set; }
            public bool TopList { get; set; }
            public bool hot { get; set; }
        }

        /// <summary>
        /// 首页展示数据
        /// </summary>
        public class HomeSorting
        {
            public string Name { get; set; }
            public sorting Sorting { get; set; }

        }

        public class BDFY
        {
            public string Name { get; set; }
            public string from { get; set; }
            public string to { get; set; }
        }

        public class OrderBy
        {
            public string Name { get; set; }
            public order Order { get; set; }
        }

        public class SearchOrders
        {
            /// <summary>
            /// 倒序
            /// </summary>
            public bool DescOrder { get; set; }
            /// <summary>
            /// 正序
            /// </summary>
            public bool AscOrder { get; set; }
        }



    }
}
