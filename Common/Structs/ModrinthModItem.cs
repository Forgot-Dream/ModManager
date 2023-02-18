using ImTools;
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
        public void AcquireFileInfo()
        {
            FileInfos = ModrinthAPI.API().GetModVersions(ModInfo.ID);
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
            return info;
        }


        /// <summary>
        /// 获取Icon
        /// </summary>
        private async void AcquireIcon()
        {
            if (IconUrl == null)
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

        private string? iconpath;
        /// <summary>
        /// 图标文件的本地地址
        /// </summary>
        public string? IconPath
        {
            get
            {
                if (iconpath == null) { AcquireIcon(); }
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
                DownloadUrl = jToken["files"]["url"].Value<string>(),
                FileID = jToken["id"].Value<string>(),
                FileName = jToken["files"]["filename"].Value<string>(),
                Name = jToken["name"].Value<string>(),
                VersionType = jToken["version_type"].Value<string>()
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

    }
}
