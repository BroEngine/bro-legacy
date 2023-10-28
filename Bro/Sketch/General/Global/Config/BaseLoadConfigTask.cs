using System.Collections.Generic;
using Bro.Sketch;

namespace Bro.Sketch
{
    public abstract class BaseLoadConfigTask : SubscribableTask<BaseLoadConfigTask>
    {
        public abstract  List<ConfigDetails> DetailsCatalog { get; set; }
        public abstract  ConfigStorageCollector Collector { get; set; }
    }
}