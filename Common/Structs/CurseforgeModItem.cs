using ModManager.Extension;
using ModManager.Utils;
using ModManager.Utils.APIs;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ModManager.Common.Structs
{
    public class CurseforgeModItem : BindableBase
    {
        enum ModLoaderType
        {
            ANY,
            FORGE,
            CAULDRON,
            LITELOADER,
            FABRIC,
            QUILT
        }

        private CurseforgeModInfo modInfo;
        /// <summary>
        /// Curseforge Mod的基本信息
        /// </summary>
        public CurseforgeModInfo ModInfo
        {
            get { return modInfo; }
            private set { modInfo = value; }
        }

        private List<Version2FileInfo> fileInfos = new();
        /// <summary>
        /// Curseforge Mod的文件列表
        /// </summary>
        public List<Version2FileInfo> FileInfos
        {
            get { return fileInfos; }
            private set { fileInfos = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// ModLoader的类型
        /// </summary>
        private List<int> ModLoaderTypes = new();

        /// <summary>
        /// 支持的游戏版本
        /// </summary>
        private List<string> SupportedGameVersions = new();

        /// <summary>
        /// 用于绑定的加载器字符串
        /// </summary>
        public string SupportedModLoader
        {
            get
            {
                var supportedmodloader = "[";
                for (int i = 0; i < ModLoaderTypes.Count; i++)
                {
                    switch (ModLoaderTypes[i])
                    {
                        case (int)ModLoaderType.FORGE: supportedmodloader += " Forge "; break;
                        case (int)ModLoaderType.FABRIC: supportedmodloader += " Fabric "; break;
                        case (int)ModLoaderType.QUILT: supportedmodloader += " Quilt "; break;
                    }
                }
                if (supportedmodloader.Length == 1)
                    supportedmodloader += " Unknown ]";
                else
                    supportedmodloader += "]";
                return supportedmodloader;
            }
        }

        /// <summary>
        /// 用于绑定的支持游戏版本
        /// </summary>
        public string SupportedVersion
        {
            get
            {
                return MinecraftVersionManager.INSTANCE.GetSupportedVersionAsString(SupportedGameVersions);
            }
        }

        /// <summary>
        /// 从json对象中序列化
        /// </summary>
        /// <param name="jToken">json对象</param>
        /// <returns>CurseforgeMod实例</returns>
        public static CurseforgeModItem fromJson(JToken jToken)
        {
            CurseforgeModItem item = new()
            {
                ModInfo = CurseforgeModInfo.fromJson(jToken)
            };

            foreach (var file in jToken["latestFilesIndexes"])
            {
                var gameversion = file["gameVersion"].Value<string>();
                if (!item.SupportedGameVersions.Contains(gameversion))
                    item.SupportedGameVersions.Add(gameversion);

                if (!file["modLoader"].IsNullOrEmpty())
                {
                    var loadertype = file["modLoader"].Value<int>();
                    if (!item.ModLoaderTypes.Contains(loadertype))
                        item.ModLoaderTypes.Add(loadertype);
                }
            }
            item.ModLoaderTypes.Sort();
            return item;
        }
        /// <summary>
        /// 要求文件信息
        /// </summary>
        public void AcquireFileInfo()
        {
            if (FileInfos.Count > 0)
                return;
            var infos = CurseforgeAPI.API().GetModFileInfos(ModInfo.ID);
            var orderedlist =
                from fileinfo in infos
                group fileinfo by fileinfo.GameVersion into newList
                orderby newList.Key descending
                select newList;
            foreach(var item in orderedlist)
            {
                var v2f = new Version2FileInfo()
                {
                    GameVersion = item.Key
                };
                foreach(var fileinfo in item)
                {
                    v2f.FileInfos.Add(fileinfo);
                }
                FileInfos.Add(v2f);
            }
        }
    }
    public class CurseforgeModInfo : BindableBase
    {
        public static CurseforgeModInfo fromJson(JToken jToken)
        {
            CurseforgeModInfo info = new()
            {
                ID = jToken["id"].Value<int>(),
                Name = jToken["name"].Value<string?>(),
                Slug = jToken["slug"].Value<string?>(),
                WebsiteURL = jToken["links"]["websiteUrl"].Value<string?>(),
                Summary = jToken["summary"].Value<string?>(),
                DownloadCount = jToken["downloadCount"].Value<Int64>(),
                DateModified = DateTime.Parse(jToken["dateModified"].Value<string?>())
            };
            if (!jToken["logo"].IsNullOrEmpty())
                info.IconURL = jToken["logo"]["thumbnailUrl"].Value<string>();

            foreach (var author in jToken["authors"])
                info.Authors.Add(author["name"].Value<string?>());

            return info;
        }

        CurseforgeModInfo()
        {
            authors = new();
        }

        /// <summary>
        /// 获取Icon
        /// </summary>
        private async void AcquireIcon()
        {
            if (string.IsNullOrEmpty(IconURL))
                return;
            var path = System.Environment.CurrentDirectory + $"/caches/curseforge/{ID}.png";
            if (System.IO.File.Exists(path))
            {
                IconPath = path;
                return;
            }
            DirectoryInfo directoryInfo = new(System.Environment.CurrentDirectory);
            directoryInfo.CreateSubdirectory("caches/curseforge");
            _ = await DownloadUtil.DownloadFromURL(IconURL, path);
            IconPath = path;
        }

        private List<string> authors;
        /// <summary>
        /// 作者列表
        /// </summary>
        public List<string> Authors
        {
            get { return authors; }
            set { authors = value; }
        }
        /// <summary>
        /// 用于绑定的的作者字符串
        /// </summary>
        public string Author
        {
            get
            {
                var ret = "";
                foreach (var author in Authors)
                    ret += " " + author;
                return ret;
            }
        }

        private string? websiteurl;
        /// <summary>
        /// 主页的URL
        /// </summary>
        public string? WebsiteURL
        {
            get { return websiteurl; }
            set { websiteurl = value; }
        }
        private string? description;
        /// <summary>
        /// Mod描述
        /// </summary>
        public string? Description
        {
            get { return description; }
            set { description = value; }
        }

        private int id;
        /// <summary>
        /// Curseforge Mod的唯一Project id
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private string? name;
        /// <summary>
        /// 模组的名字
        /// </summary>
        public string? Name
        {
            get { return name; }
            set { name = value; }
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

        private string? iconurl;
        /// <summary>
        /// 图标的远程网址
        /// </summary>
        public string? IconURL
        {
            get { return iconurl; }
            set { iconurl = value; }
        }

        private string? iconpath = "/Assets/defaulticon.png";
        /// <summary>
        /// 图标缓存之后的本地路径
        /// </summary>
        public string? IconPath
        {
            get
            {
                if (IconURL == null)
                    return iconpath;
                if (iconpath == "/Assets/defaulticon.png") { AcquireIcon(); }
                return iconpath;
            }
            private set { iconpath = value; RaisePropertyChanged(); }
        }

        private string? summary;
        /// <summary>
        /// 该项目的简介
        /// </summary>
        public string? Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        private Int64 downloadcount;
        /// <summary>
        /// 下载总数
        /// </summary>
        public Int64 DownloadCount
        {
            get { return downloadcount; }
            set { downloadcount = value; }
        }

        private DateTime datemodified;
        /// <summary>
        /// 该项目的修改时间（大概也能认为是更新时间？）
        /// </summary>
        public DateTime DateModified
        {
            get { return datemodified; }
            set { datemodified = value; }
        }


    }
    public class CurseforgeModFileInfo
    {
        public static CurseforgeModFileInfo fromJson(JToken jToken)
        {
            CurseforgeModFileInfo info = new()
            {
                FileID = jToken["id"].Value<int>(),
                DisplayName = jToken["displayName"].Value<string?>(),
                FileName = jToken["fileName"].Value<string?>(),
                ReleaseType = jToken["releaseType"].Value<int>(),
                FileLength = jToken["fileLength"].Value<Int64>(),
                DownloadURL = jToken["downloadUrl"].Value<string?>(),
                FileDate = DateTime.Parse(jToken["fileDate"].Value<string?>())
            };
            foreach(var item in jToken["sortableGameVersions"])// I don't know why CF put game version and loader type together.
            {
                if (!item["gameVersion"].IsNullOrEmpty())
                {
                    info.GameVersion = item["gameVersion"].Value<string>();
                    break;
                }
            }
            return info;
        }

        private int fileid;
        /// <summary>
        /// Curseforge Mod的文件ID
        /// </summary>
        public int FileID
        {
            get { return fileid; }
            set { fileid = value; }
        }

        private string? displayname;
        /// <summary>
        /// 发布的名字 (ep.Carpet v1.4.91 for 1.19.3)
        /// </summary>
        public string? DisplayName
        {
            get { return displayname; }
            set { displayname = value; }
        }

        private string? filename;
        /// <summary>
        /// 发布的文件名字
        /// </summary>
        public string? FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        private int releasetype;
        /// <summary>
        /// 发布的版本 1=Release（正式发布） Beta Alpha ?
        /// </summary>
        public int ReleaseType
        {
            get { return releasetype; }
            set { releasetype = value; }
        }

        private string? downloadurl;
        /// <summary>
        /// 下载链接
        /// </summary>
        public string? DownloadURL
        {
            get { return downloadurl; }
            set { downloadurl = value; }
        }

        private DateTime filedate;
        /// <summary>
        /// 该版本的发布时间
        /// </summary>
        public DateTime FileDate
        {
            get { return filedate; }
            set { filedate = value; }
        }

        private Int64 filelength;
        /// <summary>
        /// 文件大小 (byte)
        /// </summary>
        public Int64 FileLength
        {
            get { return filelength; }
            set { filelength = value; }
        }

        private string? gameversion;
        /// <summary>
        /// 该文件对应的游戏版本
        /// </summary>
        public string? GameVersion
        {
            get { return gameversion; }
            set { gameversion = value; }
        }

    }
    public class Version2FileInfo
    {
        public Version2FileInfo()
        {
            FileInfos = new();
        }
        public string? GameVersion { get; set; }
        public List<CurseforgeModFileInfo> FileInfos { get; set; }
    }
}