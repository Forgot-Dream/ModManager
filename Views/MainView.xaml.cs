using ModManager.Common.Events;
using ModManager.Views.Dialogs;
using Prism.Events;
using System.Windows;

namespace ModManager.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IEventAggregator aggregator)
        {
            InitializeComponent();
            aggregator.GetEvent<LoadingEvent>().Subscribe(args =>
            {
                Dialoghost.IsOpen = args;
                if (Dialoghost.IsOpen)
                    Dialoghost.DialogContent = new ProgressView();
            });
        }
    }
}
