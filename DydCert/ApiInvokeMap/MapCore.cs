using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ApiInvokeMap
{
    public class MapCore
    {
        private List<MapItem> items = new List<MapItem>();
        private static MapCore _instance = new MapCore();
        private int wrapmisseconds = 1000;
        private System.Threading.Thread threadtowrap = null;
        private object lockaddobj = new object();
        private MapCore()
        {
            InitWrap(wrapmisseconds);
        }
        static MapCore() { }

        public static MapCore GetInstance()
        {
            return _instance;
        }

        public void Increase(string url)
        {
            var item = GetItemOrAdd(url);
            lock (item)
            {
                item.count++;
            }
        }
        public void Decrease(string url)
        {
            var item = GetItemOrAdd(url);
            lock (item)
            {
                if (item.count > 0)
                    item.count--;
            }
        }

        private MapItem GetItemOrAdd(string url)
        {
            var item = items.FirstOrDefault(x => x.url == url);
            if (item == null)
            {
                item = AddItem(url);
            }
            return item;
        }

        private MapItem AddItem(string url)
        {
            lock (lockaddobj)
            {
                var item = items.FirstOrDefault(x => x.url == url);
                if (item != null)
                    return item;
                items.Add(item = new MapItem()
                {
                    url = url,
                    count = 0,
                    lastcount = 0
                });
                return item;
            }
        }

        private void DoWrap()
        {
            while (true)
            {
                foreach (var x in items)
                {
                    lock (x)
                    {
                        x.lastcount = x.count;
                        x.count = 0;
                    }
                }
                System.Threading.Thread.Sleep(wrapmisseconds);
            }
        }
        public void InitWrap(int miseconds)
        {
            wrapmisseconds = miseconds;
            if (threadtowrap != null)
            {
                threadtowrap.Abort();
                threadtowrap = null;
            }
            threadtowrap = new System.Threading.Thread(DoWrap);
            threadtowrap.Start();
        }
        public int GetWrapTime()
        {
            return wrapmisseconds;
        }
        public List<MapItem> GetMap()
        {
            List<MapItem> cloneitems = new List<MapItem>();
            foreach (var a in items)
            {
                cloneitems.Add(a.Clone() as MapItem);
            }
            return cloneitems;
        }
    }
}
