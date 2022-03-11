using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallEventGUI.ConvertModult;
using WallEventGUI.Model;
using WallEventGUI.WallHavenTools;
using WallHavenGui.Model;
using WallHavenGui.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using static WallEventGUI.Model.UIModel;
using static WallHavenGui.Model.FanYi;
using static WallHavenGui.Model.YouDaoFanYi;

namespace WallEventGUI.UserContent.ContentVM
{
    public class SearchContentVM: ObservableObject
    {
        WallHevenSettingResource home = new WallHevenSettingResource();
        public SearchContentVM()
        {
            if(_PurityCS==null)
                _PurityCS = UIConvert.BackLevel(home.SettingGetConfig(AppSettingArgs.SearchPurity));
            if (_CatCs == null)
                _CatCs = UIConvert.BackLevel(home.SettingGetConfig(AppSettingArgs.SearchCat));
            bool sortingresult = string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.SearchSorting));
            _Sorting = sortingresult ? new Sorting() {Random = true }: UIConvert.ConvertToSorting(UIConvert.GetEnumSorting(home.SettingGetConfig(AppSettingArgs.SearchSorting)));
            SearchClick = new RelayCommand(search);
            ApiKey = string.IsNullOrWhiteSpace(Setting.SettingGetConfig(AppSettingArgs.OpenKey))?"": Setting.SettingGetConfig(AppSettingArgs.OpenKey);
            fanyifrom = string.IsNullOrWhiteSpace(Setting.SettingGetConfig(AppSettingArgs.BaiduFanYiFrom)) ? "zh" : Setting.SettingGetConfig(AppSettingArgs.BaiduFanYiFrom);
            fanyito = string.IsNullOrWhiteSpace(Setting.SettingGetConfig(AppSettingArgs.BaiduFanYiTo)) ? "en" : Setting.SettingGetConfig(AppSettingArgs.BaiduFanYiTo);
            ScrlLoad = new RelayCommand<GridView>((se) => scrool(se));
            _SearchData = new Wallpapers();
            bool orderby = string.IsNullOrWhiteSpace(home.SettingGetConfig(AppSettingArgs.SearchOrderBy));
            _Sorder = orderby ? new SearchOrders() { DescOrder = true }:
                home.SettingGetConfig(AppSettingArgs.SearchOrderBy).ToLower() == "desc" ? new SearchOrders() { DescOrder = true } : new SearchOrders() { AscOrder = true };
           
            UpdataSearchData = new RelayCommand(update);
            _Items = new ObservableCollection<string>();
           

        }

        private ObservableCollection<string> items;

        public ObservableCollection<string> _Items
        {
            get { return items; }
            set { items = value;OnPropertyChanged(); }
        }

        /// <summary>
        /// 获取建议搜索
        /// </summary>
        private async void update()
        {
            _Items.Clear();
            Rootobject result = await FanYi.Baidu_Translate(fanyifrom, fanyito, SearchText);
            if (!string.IsNullOrWhiteSpace(SearchText))          //防止文本框为空
            {
                if (result.trans_result != null)
                {
                    foreach (var item in result.trans_result)
                    {
                        _Items.Add(item.dst);
                    }
                }
            }
            else
            {
                _Items.Add("未输入字符，也可能建议翻译服务不可用了……");
            }
        }

        public void DescRad_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;  
            if (radio.Content.ToString() == "正序")
            {
                _Order = order.asc;
                return;
            }
            _Order = order.desc;
        }


        public void Save()
        {

            PurityString = UIConvert.GetLevel(_PurityCS);
            CatString = UIConvert.GetLevel(_CatCs);
            
            SortingString = UIConvert.GetSorting<Sorting>(Sorting);
            //保存搜索配置
            WallHevenSettingResource home = new WallHevenSettingResource();
            home.SettingSetConfig(AppSettingArgs.SearchSorting, SortingString);
            home.SettingSetConfig(AppSettingArgs.SearchOrderBy, _Order.ToString());
            home.SettingSetConfig(AppSettingArgs.SearchCat, CatString);
            home.SettingSetConfig(AppSettingArgs.SearchPurity, PurityString);
        }

        FanYi fanyi = new FanYi();
        string ApiKey { get; set; }
        string PurityString { get; set; }
        string CatString { get; set; }
        string SortingString { get; set; }
        order _Order { get; set; }
        ScrollViewer SV = new ScrollViewer();
        Tools tools = new Tools();
        sorting searchSorting { get; set; }
        string fanyifrom { get; set; }
        string fanyito { get; set; }
        private void scrool(GridView se)
        {
            var listview = (VisualTreeHelper.GetChild(se, 0) as Border).Child as ScrollViewer;
            SV = listview;
            SV.ViewChanged += SV_ViewChanged;
        }

        private void SV_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            
            var sv = sender as ScrollViewer;
            var flage = sv.VerticalOffset + sv.ViewportHeight;          //已经滚动的长度
            if (sv.ExtentHeight - flage < 5 && sv.ViewportHeight != 0)
            {
                MoreData();
            }
            
        }

        private Meta meta;

        public Meta MyMeta
        {
            get { return meta; }
            set { meta = value; OnPropertyChanged(); }
        }


        private async void MoreData()
        {
            SV.ViewChanged -= SV_ViewChanged;
            _ProState = true;
            int maxpage = int.Parse(_SearchData.Meta.Last_Page);
            if (maxpage != 0 || pagesize<maxpage)
            {
                pagesize++;
                var result = await tools.GetSearchWallpaperString(searchSorting, topRange.D1, _SearchText, "", CatString, PurityString, _Order, pagesize, colors.None);
                meta = result.Meta;
                foreach (var item in result.WallpaperList)
                {
                    SearchData.WallpaperList.Add(item);
                }
            }
            SearchResultText = $"搜索 {SearchText}关键字，共搜索到{meta.Last_Page}页，已经加载{pagesize}页。";
            _ProState = false;
            SV.ViewChanged += SV_ViewChanged;
        }


        private int pagesize;

        public int PageSize
        {
            get { return pagesize; }
            set { pagesize = value; OnPropertyChanged();}
        }


        WallHevenSettingResource Setting = new WallHevenSettingResource();
        public async void search()
        {
            if(pagesize<1)pagesize = 1;
            _ProState = true;
            PurityString = UIConvert.GetLevel(_PurityCS);
            CatString = UIConvert.GetLevel(_CatCs);
            SortingString = UIConvert.GetSorting<Sorting>(Sorting);
            
            tools.OpenKey = string.IsNullOrWhiteSpace(ApiKey)||ApiKey == null ? false : true;
            tools.Key = tools.OpenKey ? ApiKey : null;
            searchSorting = UIConvert.GetEnumSorting(SortingString);
            
            var result = await tools.GetSearchWallpaperString(searchSorting, topRange.D1, _SearchText,"", CatString, PurityString, _Order, pagesize, colors.None);
            meta = result.Meta;
            SearchResultText = $"搜索 {SearchText}关键字，共搜索到{meta.Last_Page}页，已经加载{pagesize}页。";
            _SearchData = result;
            _ProState = false;
        }


        

        
        private LevelCs PurityCS;

        public LevelCs _PurityCS
        {
            get { return PurityCS; }
            set { PurityCS = value;OnPropertyChanged(); }
        }

        private LevelCs CatCS;

        public LevelCs _CatCs
        {
            get { return CatCS; }
            set { CatCS = value; OnPropertyChanged(); }
        }

        private string SearchText;

        public string _SearchText
        {
            get { return SearchText; }
            set { SearchText = value;OnPropertyChanged(); }
        }

        public RelayCommand UpdataSearchData { get; set; }

        private Sorting Sorting;

        public Sorting _Sorting
        {
            get { return Sorting; }
            set { Sorting = value; OnPropertyChanged(); }
        }

        private Wallpapers SearchData;

        public Wallpapers _SearchData
        {
            get { return SearchData; }
            set { SearchData = value;OnPropertyChanged(); }
        }

        private SearchOrders SOrder;

        public SearchOrders _Sorder
        {
            get { return SOrder; }
            set { SOrder = value;OnPropertyChanging(); }
        }


        private string _SearchResultText;

        public string SearchResultText
        {
            get { return _SearchResultText; }
            set { _SearchResultText = value;OnPropertyChanged(); }
        }




        public RelayCommand SearchClick { get; set; }
        public RelayCommand<GridView> ScrlLoad { get; private set; }


        private bool ProState;

        public bool _ProState
        {
            get { return ProState; }
            set { ProState = value; OnPropertyChanged(); }
        }


        public void Changed(object sender, SelectionChangedEventArgs e)
        {
            Wallpaper wall = (Wallpaper)(sender as GridView).SelectedItem;
            if (wall != null)
            {
                Frame rootFrame = Window.Current.Content as Frame;      //顶部Frame
                rootFrame.Navigate(typeof(WallpaperPage), wall, new DrillInNavigationTransitionInfo());

            }
        }
    }
}
