using Microsoft.Web.WebView2.Core;
using ModManager.Common.Events;
using Prism.Events;
using System.Diagnostics;
using System.Windows.Controls;

namespace ModManager.Views
{
    /// <summary>
    /// CurseforgeModView.xaml 的交互逻辑
    /// </summary>
    public partial class CurseforgeModView : UserControl
    {
        public CurseforgeModView(IEventAggregator aggregator)
        {
            InitializeComponent();
            aggregator.GetEvent<SetCurseforgeModBodyEvent>().Subscribe(async (html) =>
            {
                await webview.EnsureCoreWebView2Async();
                webview.CoreWebView2.NavigateToString(html);
            });
            webview.NavigationStarting += ToLocalView;
        }

        private void ToLocalView(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith("http") || e.Uri.StartsWith("linkout"))
            {
                Debug.WriteLine($"Open Page on Local: {e.Uri}");
                Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = e.Uri });
                e.Cancel = true;
            }
        }
    }
}
