using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.WallHavenTools;
using Windows.Storage;

namespace WallEventGUI.Model
{
    public  class SettingResource
    {
        public SettingResource()
        {
            Load();
        }
        public static ApplicationDataContainer localSettings { get; set; }
        public void Load()
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }
    }

    public interface IResource
    {
        
        bool Exists(string key);
        Task<bool> KeyExists(string key,string id= "28jp89");
    }

    public class WallHevenSettingResource : SettingResource, IResource
    {
        /// <summary>
        /// 密钥检查
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="id">一个壁纸id</param>
        /// <returns></returns>
        public async Task<bool> KeyExists(string key, string id = "28jp89")
        {
            Tools tools = new Tools();
            tools.Key = key;
            tools.OpenKey = true;
            var cll = await tools.GetUserConfigAsync();
            if (cll == null) return false;
            return true;
        }


        public void SettingSetConfig(string key, string word)
        {
            localSettings.Values[key] = word;
        }

        public string SettingGetConfig(string key)
        {
            var txt = (string)localSettings.Values[key];
            return txt;
        }

        public bool Exists(string key)
        {   
            bool blle =  localSettings.Values.ContainsKey(key);
            return blle;
        }

        public bool DeleteKey(string key)
        {
            localSettings.Values.Remove(key);
            return true;
        }

    }
}
