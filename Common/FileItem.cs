namespace ModManager.Common
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
            return $"{filename}({gameVersion})";
        }

    }
}
