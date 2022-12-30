using ModManager.Utils;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;

namespace ModManager.Common.Structs
{
    public class CurseforgeModItem : BindableBase
    {
        private CurseforgeModInfo modInfo;
        /// <summary>
        /// Curseforge Mod的基本信息
        /// </summary>
        public CurseforgeModInfo ModInfo
        {
            get { return modInfo; }
            private set { modInfo = value; }
        }

        private List<CurseforgeModFileInfo> fileInfos;
        /// <summary>
        /// Curseforge Mod的文件列表
        /// </summary>
        public List<CurseforgeModFileInfo> FileInfos
        {
            get { return fileInfos; }
            private set { fileInfos = value; }
        }

        /// <summary>
        /// 从json对象中格式化
        /// </summary>
        /// <param name="jToken">json对象</param>
        /// <returns>CurseforgeMod实例</returns>
        public static CurseforgeModItem fromJson(JToken jToken)
        {
            CurseforgeModItem item = new()
            {
                ModInfo = CurseforgeModInfo.fromJson(jToken)
            };
            return item;
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
                DownloadCount = jToken["downloadCount"].Value<Int64>()
            };
            if (jToken["logo"].HasValues)
                info.IconURL = jToken["logo"]["thumbnailUrl"].Value<string>();

            foreach (var author in jToken["authors"])
                info.Authors.Add(author["name"].Value<string?>());

            return info;
        }

        CurseforgeModInfo()
        {
            authors = new();
        }

        private async void AcquireIcon()
        {
            if (IconURL == null)
                return;
            var path = System.Environment.CurrentDirectory + $"/caches/curseforge/{this.ID}.png";
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
                    ret += " | " + author;
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

        private string? iconpath;
        /// <summary>
        /// 图标缓存之后的本地路径
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

    }
}
