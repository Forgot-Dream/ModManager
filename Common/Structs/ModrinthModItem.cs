using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Common.Structs
{
    public class ModrinthModItem
    {
        public ModrinthModInfo ModInfo;
        public List<ModrinthModFileInfo> FileInfos;

    }
    public class ModrinthModInfo
    {
        private string slug;
        /// <summary>
        /// 唯一字符串标识符
        /// </summary>
        public string Slug
        {
            get { return slug; }
            set { slug = value; }
        }

        private string title;
        /// <summary>
        /// 名称
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string description;
        /// <summary>
        /// 一个简短的描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string id;
        /// <summary>
        /// 项目的唯一识别ID(string)
        /// </summary>
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string author;
        /// <summary>
        /// 作者
        /// </summary>
        public string Author
        {
            get { return author; }
            set { author = value; }
        }


        private string iconurl;
        /// <summary>
        /// 图标文件的远程地址
        /// </summary>
        public string IconUrl
        {
            get { return iconurl; }
            set { iconurl = value; }
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


    }
    public class ModrinthModFileInfo
    {
        private string fileid;
        /// <summary>
        /// 对应文件的ID
        /// </summary>
        public string FileID
        {
            get { return fileid; }
            set { fileid = value; }
        }

        private string name;
        /// <summary>
        /// 展示名字
        /// </summary>
        public string Name
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

        private string versiontype;
        /// <summary>
        /// 发布类型 release beta alpha
        /// </summary>
        public string VersionType
        {
            get { return versiontype; }
            set { versiontype = value; }
        }

        private string downloadurl;
        /// <summary>
        /// 下载链接
        /// </summary>
        public string DownloadUrl
        {
            get { return downloadurl; }
            set { downloadurl = value; }
        }

        private string filename;
        /// <summary>
        /// 文件名字
        /// </summary>
        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

    }
}
