using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiInvokeMap
{
    public class MapItem : ICloneable
    {
        public string url { get; set; }
        public long count { get; set; }
        public long lastcount { get; set; }

        public object Clone()
        {
            return new MapItem()
            {
                url = url,
                count = count,
                lastcount = lastcount
            };
        }
    }
}
