namespace ModManager.Common.Structs
{
    public class FileItem
    {
        public string? gameVersion { get; set; }
        public string? fileUrl { get; set; }
        public int fileId { get; set; }
        public string? filename { get; set; }

        public override string ToString()
        {
            if (filename == null)
                return string.Empty;
            if(gameVersion == null) {
                return $"{filename}";
            }
            return $"{filename}({gameVersion})";//这里原来这样写如果导入本地文件version后面会有 "()"
        }

    }
}
