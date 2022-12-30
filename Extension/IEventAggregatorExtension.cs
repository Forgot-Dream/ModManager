using ModManager.Common.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModManager.Extension
{
    public static class IEventAggregatorExtension
    {
        public static void ShowProgressBar(this IEventAggregator aggregator,bool show) {
            aggregator.GetEvent<LoadingEvent>().Publish(show);
        }
    }
}
