using Microsoft.Web.WebView2.Core;
using ModManager.Common.Events;
using Prism.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace ModManager.Views
{
    /// <summary>
    /// ModrinthModView.xaml 的交互逻辑
    /// </summary>
    public partial class ModrinthModView : UserControl
    {
        public ModrinthModView(IEventAggregator aggregator)
        {
            InitializeComponent();
            aggregator.GetEvent<SetModrinthModBodyEvent>().Subscribe(async (html) =>
            {
                await webview.EnsureCoreWebView2Async();
                webview.CoreWebView2.NavigateToString(html);
            });
            webview.NavigationStarting += ToLocalView;
        }

        private void ToLocalView(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (e.Uri.StartsWith("http"))
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = e.Uri });
                e.Cancel= true;
            }
        }
    }
}
