using ModManager.Extension;
using ModManager.Utils;
using ModManager.Utils.APIs;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModManager.Common.Structs
{
    public class ModrinthModItem
    {
        /// <summary>
        /// Mod的信息
        /// </summary>
        public ModrinthModInfo ModInfo { get; set; }
        /// <summary>
        /// 文件信息的列表
        /// </summary>
        public List<ModrinthModFileInfo> FileInfos { get; set; }

        public static ModrinthModItem fromJson(JToken jToken)
        {
            ModrinthModItem item = new()
            {
                ModInfo = ModrinthModInfo.fromJson(jToken)
            };
            return item;
        }

        public string SupportedVersion => MinecraftVersionManager.INSTANCE.GetSupportedVersionAsString(ModInfo.SupportedVersion);

        /// <summary>
        /// 要求文件信息
        /// </summary>
        public void AcquireDetailedInfo()
        {
            var fileinfos = ModrinthAPI.API().GetModVersions(ModInfo.ID);
            if (fileinfos != null)
                FileInfos = fileinfos;//要求文件信息

            var info = ModrinthAPI.API().GetModBodyFromID(ModInfo.ID);
            if (info != null)
                ModInfo.Body = info;//要求详细信息
        }
    }
    public class ModrinthModInfo : BindableBase
    {
        /// <summary>
        /// 从Json反序列化Modrinth的Mod基础信息
        /// </summary>
        /// <param name="jToken">Json对象</param>
        /// <returns>Modrinth的Mod基础信息对象</returns>
        public static ModrinthModInfo fromJson(JToken jToken)
        {
            ModrinthModInfo info = new()
            {
                Author = jToken["author"].Value<string?>(),
                DateModified = DateTime.Parse(jToken["date_modified"].Value<string?>()),
                Description = jToken["description"].Value<string?>(),
                DownloadCount = jToken["downloads"].Value<int>(),
                IconUrl = jToken["icon_url"].Value<string?>(),
                ID = jToken["project_id"].Value<string?>(),
                Slug = jToken["slug"].Value<string?>(),
                Title = jToken["title"].Value<string?>(),
                SupportedVersion = jToken["versions"].Values<string>().ToList()
            };
            //写入server / client的需求数据
            switch (jToken["client_side"].Value<string>())
            {
                case "required":
                    info.ClientSide = "必需";
                    break;
                case "optional":
                    info.ClientSide = "可选";
                    break;
                case "unsupported":
                    info.ClientSide = "不支持";
                    break;
            }
            switch (jToken["server_side"].Value<string>())
            {
                case "required":
                    info.ServerSide = "必需";
                    break;
                case "optional":
                    info.ServerSide = "可选";
                    break;
                case "unsupported":
                    info.ServerSide = "不支持";
                    break;
            }
            info.Categorys = new();
            foreach (var item in jToken["categories"])
                info.Categorys.Add(new ModrinthCategory() { Name = item.Value<string>() });
            return info;
        }


        /// <summary>
        /// 获取Icon
        /// </summary>
        private async void AcquireIcon()
        {
            if (string.IsNullOrEmpty(IconUrl))
                return;
            var path = System.Environment.CurrentDirectory + $"/caches/modrinth/{ID}.png";
            if (System.IO.File.Exists(path))
            {
                IconPath = path;
                return;
            }
            DirectoryInfo directoryInfo = new(System.Environment.CurrentDirectory);
            directoryInfo.CreateSubdirectory("caches/modrinth");
            _ = await DownloadUtil.DownloadFromURL(IconUrl, path);
            IconPath = path;
        }

        private string? slug;
        /// <summary>
        /// 唯一字符串标识符
        /// </summary>
        public string? Slug
        {
            get { return slug; }
            set { slug = value; }
        }

        private string? title;
        /// <summary>
        /// 名称
        /// </summary>
        public string? Title
        {
            get { return title; }
            set { title = value; }
        }

        private string? description;
        /// <summary>
        /// 一个简短的描述
        /// </summary>
        public string? Description
        {
            get { return description; }
            set { description = value; }
        }

        private string? id;
        /// <summary>
        /// 项目的唯一识别ID(string?)
        /// </summary>
        public string? ID
        {
            get { return id; }
            set { id = value; }
        }

        private string? author;
        /// <summary>
        /// 作者
        /// </summary>
        public string? Author
        {
            get { return author; }
            set { author = value; }
        }


        private string? iconurl;
        /// <summary>
        /// 图标文件的远程地址
        /// </summary>
        public string? IconUrl
        {
            get { return iconurl; }
            set { iconurl = value; }
        }

        private string? iconpath = "/Assets/defaulticon.png";
        /// <summary>
        /// 图标文件的本地地址
        /// </summary>
        public string? IconPath
        {
            get
            {
                if (IconUrl == null)
                    return iconpath;
                if (iconpath == "/Assets/defaulticon.png") { AcquireIcon(); }
                return iconpath;
            }
            private set { iconpath = value; RaisePropertyChanged(); }
        }


        private DateTime datemodified;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime DateModified
        {
            get { return datemodified; }
            set { datemodified = value; }
        }

        private int downloadcount;
        /// <summary>
        /// 下载次数统计
        /// </summary>
        public int DownloadCount
        {
            get { return downloadcount; }
            set { downloadcount = value; }
        }

        private List<string?> supportedversion;
        /// <summary>
        /// 支持的MC版本
        /// </summary>
        public List<string?> SupportedVersion
        {
            get { return supportedversion; }
            set { supportedversion = value; }
        }

        private string? serverside;
        /// <summary>
        /// 服务器侧支持情况 -1=不支持 0=可选 1=必需
        /// </summary>
        public string? ServerSide
        {
            get { return serverside; }
            set { serverside = value; }
        }

        private string? clientside;
        /// <summary>
        /// 客户端侧支持情况 -1=不支持 0=可选 1=必需
        /// </summary>
        public string? ClientSide
        {
            get { return clientside; }
            set { clientside = value; }
        }

        private List<ModrinthCategory> categorys;
        /// <summary>
        /// 标签
        /// </summary>
        public List<ModrinthCategory> Categorys
        {
            get { return categorys; }
            set { categorys = value; }
        }

        private string? body;
        /// <summary>
        /// 详细信息 Markdown格式
        /// </summary>
        public string? Body
        {
            get { return body; }
            set { body = value; }
        }



    }
    public class ModrinthModFileInfo
    {
        /// <summary>
        /// 对Json的反序列化成Modrinth的文件信息对象
        /// </summary>
        /// <param name="jToken">Json对象</param>
        /// <returns>Modrinth的文件信息对象</returns>
        public static ModrinthModFileInfo fromJson(JToken jToken)
        {
            ModrinthModFileInfo info = new()
            {
                DatePublished = DateTime.Parse(jToken["date_published"].Value<string>()),
                DownloadUrl = jToken["files"][0]["url"].Value<string>(),
                FileID = jToken["id"].Value<string>(),
                FileName = jToken["files"][0]["filename"].Value<string>(),
                Name = jToken["name"].Value<string>(),
                VersionType = jToken["version_type"].Value<string>(),
                VersionNumber = jToken["version_number"].Value<string>(),
                GameVersions = jToken["game_versions"].Values<string>().ToList(),
                Loaders = jToken["loaders"].Values<string>().ToList()
            };
            return info;
        }

        private string? fileid;
        /// <summary>
        /// 对应文件的ID
        /// </summary>
        public string? FileID
        {
            get { return fileid; }
            set { fileid = value; }
        }

        private string? name;
        /// <summary>
        /// 展示名字
        /// </summary>
        public string? Name
        {
            get { return name; }
            set { name = value; }
        }

        private DateTime datepublished;
        /// <summary>
        /// 此文件的发布时间
        /// </summary>
        public DateTime DatePublished
        {
            get { return datepublished; }
            set { datepublished = value; }
        }

        private string? versiontype;
        /// <summary>
        /// 发布类型 release beta alpha
        /// </summary>
        public string? VersionType
        {
            get { return versiontype; }
            set { versiontype = value; }
        }

        private string? downloadurl;
        /// <summary>
        /// 下载链接
        /// </summary>
        public string? DownloadUrl
        {
            get { return downloadurl; }
            set { downloadurl = value; }
        }

        private string? filename;
        /// <summary>
        /// 文件名字
        /// </summary>
        public string? FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        private List<string?>? loaders;
        /// <summary>
        /// 加载器
        /// </summary>
        public List<string?>? Loaders
        {
            get { return loaders; }
            set { loaders = value; }
        }

        private List<string?>? gameversions;
        /// <summary>
        /// 支持的游戏版本
        /// </summary>
        public List<string?>? GameVersions
        {
            get { return gameversions; }
            set { gameversions = value; }
        }

        private string? versionnumber;
        /// <summary>
        /// 版本号
        /// </summary>
        public string? VersionNumber
        {
            get { return versionnumber; }
            set { versionnumber = value; }
        }

        /// <summary>
        /// 用于绑定的Loader字符串
        /// </summary>
        public string Loader
        {
            get
            {
                if (Loaders == null || Loaders.Count == 0)
                    return string.Empty;
                else if (Loaders.Count == 1)
                    return Loaders.FirstOrDefault();
                return string.Join(",", Loaders);
            }
        }

        /// <summary>
        /// 用于绑定的Gameversion字符串
        /// </summary>
        public string GameVersion
        {
            get
            {
                if (GameVersions == null || GameVersions.Count == 0)
                    return string.Empty;
                else if (GameVersions.Count == 1)
                    return GameVersions.FirstOrDefault();
                else
                    return $"{GameVersions.FirstOrDefault()}-{GameVersions.Last()}";
            }
        }


    }
    public class ModrinthCategory
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Icon
        {
            get
            {
                return $"/Assets/{Name}.png";
            }
        }

    }
}
