using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModManager.Utils
{
    public static class DownloadUtil
    {
        public static async Task<bool> DownloadFromURL(string url, string save)
        {
            if (!File.Exists(save))
            {
                Debug.WriteLine($"Download : {url}");
                var http = new HttpClient();
                var tmp = save + ".tmp";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("user-agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.102 Safari/537.36 Edg/104.0.1293.70");
                var response = await http.SendAsync(request);
                try
                {
                    response.EnsureSuccessStatusCode();
                    using (var fs = File.Open(tmp, FileMode.Create))
                    {
                        using var ms = await response.Content.ReadAsStreamAsync();
                        await ms.CopyToAsync(fs);
                    }
                    File.Move(tmp, save);
                }
                catch
                {
                    if (File.Exists(tmp))
                    {
                        File.Delete(tmp);
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
