using Prism.Mvvm;
using System.Collections.Generic;

namespace ModManager.Common
{
    public class SourceItem : BindableBase
    {
        public SourceItem()
        {
            VersionList = new List<FileItem>();
        }

        //Comment URL Type Version CurseforgeID VersionList Name   
        private string? comment;

        public string? Comment
        {
            get { return comment; }
            set { comment = value; RaisePropertyChanged(); }
        }

        private string? url;

        public string? URL
        {
            get { return url; }
            set { url = value; RaisePropertyChanged(); }
        }

        private string? type;

        public string? Type
        {
            get { return type; }
            set { type = value; RaisePropertyChanged(); }
        }


        private FileItem? version;

        public FileItem? Version
        {
            get { return version; }
            set { version = value; RaisePropertyChanged(); }
        }

        private string? curseforgeid;

        public string? CurseforgeID
        {
            get { return curseforgeid; }
            set { curseforgeid = value; RaisePropertyChanged(); }
        }

        private List<FileItem>? versionlist;

        public List<FileItem>? VersionList
        {
            get { return versionlist; }
            set { versionlist = value; RaisePropertyChanged(); }
        }

        private string? name;

        public string? Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }

        private bool isselected;

        public bool IsSelected
        {
            get { return isselected; }
            set { isselected = value; RaisePropertyChanged(); }
        }

    }
}
