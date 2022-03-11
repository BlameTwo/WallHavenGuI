using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.WallHavenTools;
using static WallEventGUI.Model.UIModel;

namespace WallEventGUI.ConvertModult
{
    public static class UIConvert
    {
        /// <summary>
        /// 对于Level分级的反向转换
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetLevel(LevelCs level)
        {
            string a = "", b = "", c = "";
            switch (level.Level1)
            {
                case true:
                    a = "1";
                    break;
                case false:
                    a = "0";
                    break;
            }
            switch (level.Level2)
            {
                case true:
                    b = "1";
                    break;
                case false:
                    b = "0";
                    break;
            }
            switch (level.Level3)
            {
                case true:
                    c = "1";
                    break;
                case false:
                    c = "0";
                    break;
            }
            return a + b + c;
        }

        /// <summary>
        /// 对于Level分级的反向转换
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static LevelCs  BackLevel(string key)
        {
            if (key == null)
                return new LevelCs() { Level1 = true };
            if (key.Length == 3 || string.IsNullOrWhiteSpace(key))
            {
                LevelCs levelCs = new LevelCs();
                int a = int.Parse(key.Substring(0, 1));
                int b = int.Parse(key.Substring(1, 1));
                int c = int.Parse(key.Substring(2, 1));
                levelCs.Level1 = System.Convert.ToBoolean(a);
                levelCs.Level2 = System.Convert.ToBoolean(b);
                levelCs.Level3 = System.Convert.ToBoolean(c);
                return levelCs;
            }
            else
            {
                return new LevelCs() { Level1 = true };
            }
        }



        /// <summary>
        /// 反射获得类的值，留着有用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetSorting<T>(T model)
        {
            Type t = model.GetType();
            PropertyInfo[] propertyInfos = t.GetProperties();
            foreach (PropertyInfo item in propertyInfos)
            {
                bool value =Convert.ToBoolean(item.GetValue(model, null));
                if(value == true)
                {
                    return item.Name;
                }
            }
            return null;
        }

        public static Sorting ConvertToSorting(sorting sorting)
        {
            switch (sorting)
            {
                case sorting.date_added:
                    return new Sorting() { Data_Added = true };
                case sorting.relevance:
                    return new Sorting() { Relevance = true };
                case sorting.random:
                    return new Sorting() { Random = true };
                case sorting.views:
                    return new Sorting() { Views = true };
                case sorting.favorites:
                    return new Sorting() { Favorites = true };
                case sorting.toplist:
                    return new Sorting() { TopList = true };
                case sorting.hot:
                    return new Sorting() { hot = true };
                default:
                    return new Sorting() { Relevance=true };
            }
        }

        public static sorting GetEnumSorting(string str)
        {
            switch (str.ToLower())
            {
                case "data_added":
                    return sorting.date_added;
                case "relevance":
                    return sorting.relevance;
                case "random":
                    return sorting.random;
                case "views":
                    return sorting.views;
                case "favorites":
                    return sorting.favorites;
                case "topList":
                    return sorting.toplist;
                case "hot":
                    return sorting.hot;
                default:        //默认返回相关
                    return sorting.relevance;
            }
        }




        public static order GetOrder(SearchOrders searchOrders)
        {
            if(searchOrders.AscOrder == true)
            {
                return order.asc;
            }else if(searchOrders.DescOrder == true)
            {
                return order.desc;
            }
            return order.desc;
        }
    }
}
