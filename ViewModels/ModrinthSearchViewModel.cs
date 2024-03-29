﻿using ModManager.Common.Structs;
using ModManager.Extension;
using ModManager.Utils;
using ModManager.Utils.APIs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModManager.ViewModels
{
    public class ModrinthSearchViewModel : BindableBase
    {
        private readonly ModrinthAPI Api;
        private readonly IEventAggregator aggregator;
        private readonly IRegionManager regionManager;
        private int lastcalledindex = -1;
        private string searchfilter;
        private MinecraftGameVersion? gameversion;
        private int classid;
        private string index = "relevance";//默认以相关度排名
        private List<ModrinthModItem> moditems;
        private List<MinecraftGameVersion> gameversions;

        /// <summary>
        /// 搜索的字符串
        /// </summary>
        public string searchFilter { get { return searchfilter; } set { searchfilter = value; RaisePropertyChanged(); } }
        /// <summary>
        /// 游戏版本
        /// </summary>
        public MinecraftGameVersion? GameVersion { get { return gameversion; } set { gameversion = value; RaisePropertyChanged(); } }
        /// <summary>
        /// 大类ID
        /// </summary>
        public int classId { get { return classid; } set { classid = value; RaisePropertyChanged(); } }
        /// <summary>
        /// 排序方式
        /// </summary>
        public string Index { get { return index; } set { index = value; RaisePropertyChanged(); } }
        /// <summary>
        /// 游戏版本的列表 在初始化的时候从CurseforgeApi拿数据，获取之前从本地缓存拿数据
        /// </summary>
        public List<MinecraftGameVersion> GameVersions { get { return gameversions; } private set { gameversions = value; RaisePropertyChanged(); } }
        public List<ModrinthModItem> ModItems
        {
            get { return moditems; }
            set { moditems = value; RaisePropertyChanged(); }
        }

        ModrinthSearchViewModel(IEventAggregator aggregator, IRegionManager regionManager)
        {
            this.aggregator = aggregator;
            this.regionManager = regionManager;
            Api = ModrinthAPI.API();
            SearchCommand = new DelegateCommand<object>(Search);
            ResetCommand = new DelegateCommand<object>(Reset);
            ViewDetailsCommand = new DelegateCommand<object>(ViewDetails);
            Init();
            Search(null); //进入的时候先加载一波mod
        }

        async void Init() //加载部分数据
        {
            await Task.Run(() =>
            {
                GameVersions = MinecraftVersionManager.INSTANCE.GetMajorVersion();
            });
        }
        public DelegateCommand<object> SearchCommand { get; private set; }
        public DelegateCommand<object> ResetCommand { get; private set; }
        public DelegateCommand<object> ViewDetailsCommand { get; private set; }

        async void Search(object? obj)
        {
            aggregator.ShowProgressBar(true);
            ModItems = await Api.SearchMods(searchFilter, Index, null, null);
            aggregator.ShowProgressBar(false);
        }
        void Reset(object? obj) { searchFilter = ""; GameVersion = null; classId = 0; Index = "relevance"; }

        async void ViewDetails(object index)
        {
            if (lastcalledindex != (int)index)
            {
                aggregator.ShowProgressBar(true);
                await Task.Run(() => { ModItems[(int)index].AcquireDetailedInfo(); });
                aggregator.ShowProgressBar(false);
                var param = new NavigationParameters
            {
                { "ModItem", ModItems[(int)index] }
            };
                regionManager.RequestNavigate("MainViewRegion", "ModrinthModView", param);
                lastcalledindex = (int)index;
                return;
            }
            regionManager.Regions["MainViewRegion"].RequestNavigate("ModrinthModView");
        }
    }
}