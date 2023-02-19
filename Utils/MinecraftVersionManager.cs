using ModManager.Common.Structs;
using ModManager.Utils.APIs;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ModManager.Utils
{
    public class MinecraftVersionManager
    {
        private static MinecraftVersionManager MinecraftVersionManagerInstance;
        /// <summary>
        /// 游戏版本的列表
        /// </summary>
        private List<MinecraftGameVersion> GameVersions { get; set; }
        /// <summary>
        /// 稳定版游戏版本列表
        /// </summary>
        private List<MinecraftGameVersion> MajorGameVersions;
        /// <summary>
        /// 实例接口
        /// </summary>
        public static MinecraftVersionManager INSTANCE
        {
            get
            {
                MinecraftVersionManagerInstance ??= new MinecraftVersionManager();
                return MinecraftVersionManagerInstance;
            }
        }

        MinecraftVersionManager() //优先级Modrinth>Curseforge
        {
            GameVersions ??= ModrinthAPI.API().GetMinecraftVersionList();
            GameVersions ??= CurseforgeAPI.API().GetMinecraftVersionList();
            MajorGameVersions = GameVersions.Where(version => version.version_type == "release").ToList();
        }

        /// <summary>
        /// 获取主要版本
        /// </summary>
        /// <returns>主要版本的列表</returns>
        public List<MinecraftGameVersion> GetMajorVersion()
        {
            return MajorGameVersions;
        }

        /// <summary>
        /// 格式化支持版本的字符串
        /// </summary>
        /// <param name="SupportedVersionList">支持游戏版本的列表</param>
        /// <returns>格式化字符串</returns>
        public string GetSupportedVersionAsString(List<string> SupportedVersionList)
        {
            Dictionary<int, string> Data = new();
            var StringGameVersions = MajorGameVersions.Select(x => x.version).ToList();
            foreach (var item in SupportedVersionList)
            {
                var index = StringGameVersions.IndexOf(item);
                if (index == -1)
                    continue;
                Data.Add(index, item);
            }

            var sortedversion = Data.OrderByDescending(x => x.Key).ToList();

            int lastversion = 0;
            int version;
            StringBuilder supportversion = new();
            for (version = 1; version < sortedversion.Count; version++)
            {
                if (sortedversion[version - 1].Key - sortedversion[version].Key > 1)
                {
                    if (lastversion == version - 1)
                    {
                        supportversion.Append($"| {sortedversion[lastversion].Value} ");
                    }
                    else
                    {
                        supportversion.Append($"| {sortedversion[lastversion].Value}-{sortedversion[version - 1].Value} ");
                    }
                    lastversion = version;
                }
            }
            if (lastversion != version && sortedversion.Count > 0)
                supportversion.Append($"| {sortedversion[lastversion].Value}-{sortedversion[version - 1].Value} |");
            return supportversion.ToString();

        }
    }
}
