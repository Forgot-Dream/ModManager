using System;

namespace ModManager.Common.Structs
{
    public class MinecraftGameVersion
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string? version { get; set; }
        /// <summary>
        /// 版本类型
        /// </summary>
        public string? version_type { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 是否是主要版本
        /// </summary>
        public bool major { get; set; }

    }
}
