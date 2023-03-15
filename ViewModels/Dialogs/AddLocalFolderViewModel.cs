using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using ModManager.Common;
using ModManager.Common.Events;
using ModManager.Common.Structs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.IO;
using System.Linq;
using static ModManager.Utils.MurmurHash2;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ModManager.ViewModels.Dialogs
{
    public class AddLocalFolderViewModel : BindableBase, IDialogHostAware
    {
        private readonly IEventAggregator aggregator;

        public string DialogHostName { get; set; }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand LoadCommand { get; set; }
        private string url;

        public string URL
        {
            get { return url; }
            set { url = value; RaisePropertyChanged(); }
        }

        public string Type { get; set; }


        public AddLocalFolderViewModel(IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
        }
        public void OnDialogOpend(IDialogParameters parameters)
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            LoadCommand = new DelegateCommand(Load);
        }

        private void Load()
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog("请选择要添加的本地文件夹...");
            openFileDialog.IsFolderPicker = true;
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Cancel)
                return;
            URL = openFileDialog.FileName;
        }

        private void Cancel()
        {
            DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.Cancel));
        }
        private void Save()
        {
            string[] allurl = GetAllUrl(URL);
            allurl = allurl.OrderBy(s => new FileInfo(s).Name).ToArray();
            foreach (string url in allurl)
            {
                SaveFile(url);
            }
        }

        private string[] GetAllUrl(string Folderurl)//Find URLs for all files under the given URL
        {
            string[] output = new string[0];
            if (System.IO.Directory.Exists(Folderurl))
            {
                string[] subFiles = Directory.GetFiles(Folderurl);
                output = output.Concat(subFiles).ToArray();
                string[] subFolders = Directory.GetDirectories(Folderurl);
                foreach (string folders in subFolders)
                {
                    output = output.Concat(GetAllUrl(folders)).ToArray();
                }
                return output;
            }
            return null;
            
        }

        private void SaveFile(string fileURL)//URL for the file, not the folder
        {
            if (Type == null || fileURL == null)
                return;
            FileInfo fileInfo = new FileInfo(fileURL);
            ModItem sourceItem = new ModItem() { Type = "本地Mod", URL = fileURL, Comment = fileInfo.Name };
            sourceItem.VersionList.Add(new FileItem() { filename = ExtractNumberSubstring(fileInfo.Name), fileUrl = fileInfo.FullName });
            sourceItem.Version = sourceItem.VersionList.Last();
            aggregator.GetEvent<AddItemEvent>().Publish(sourceItem);
            if (DialogHost.IsDialogOpen("Root") == true)
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
        }
        static string ExtractNumberSubstring(string input) {//截取数字作为版本，虽然不是很靠谱但只能这样了
            Regex regex = new Regex(@"\d+");
            MatchCollection matches = regex.Matches(input);
            if (matches.Count == 0) {
                return "";
            }
            int startIndex = matches[0].Index;
            int endIndex = matches[matches.Count - 1].Index + matches[matches.Count - 1].Length;
            return input.Substring(startIndex, endIndex - startIndex);
        }  
    }
}