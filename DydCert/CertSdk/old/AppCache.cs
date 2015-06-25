using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk.old.CertCenter
{
    public class AppCache
    {
        private List<AppModel> cacheapps;
        public AppCache()
        {
            System.Runtime.Caching.MemoryCache mcache = System.Runtime.Caching.MemoryCache.Default;
            List<AppModel> _cacheapps = mcache.Get(getcachename) as List<AppModel>;
            if (_cacheapps == null)
            {
                _cacheapps = new List<AppModel>();
                bool r = mcache.Add(getcachename, _cacheapps, new DateTimeOffset(DateTime.Now.AddMinutes(expiresminutes)));
                if (!r)
                {
                    throw new Exception("初始化缓存失败。");
                }
            }
            cacheapps = _cacheapps.CloneList();
        }

        public AppModel GetApp(string appid)
        {
            if (cacheapps != null)
            {
                AppModel m = cacheapps.FirstOrDefault(x => x.appid == appid);
                return m;
            }
            return null;
        }

        private void SetCache()
        {
            System.Runtime.Caching.MemoryCache mcache = System.Runtime.Caching.MemoryCache.Default;
            if (cacheapps == null)
                cacheapps = new List<AppModel>();
            if (mcache.Get(getcachename) != null)
            {
                mcache.Remove(getcachename);
            }

            bool r = mcache.Add(getcachename, cacheapps.CloneList(), new DateTimeOffset(DateTime.Now.AddMinutes(expiresminutes)));
            if (!r)
            {
                mcache = null;
            }
        }

        private string getcachename
        {
            get
            {
                return "CertCenter.app";
            }
        }

        private int expiresminutes
        {
            get
            {
                return 1 * 24 * 60;
            }
        }

        public void AddApp(AppModel appmodel)
        {
            cacheapps.Add(appmodel);
            SetCache();
        }
    }

    public class AppModel : ICloneable
    {
        public string appid { get; set; }

        public string appsecret { get; set; }

        public ServiceCertType apptype { get; set; }

        public object Clone()
        {
            return new AppModel()
            {
                appid = this.appid,
                appsecret = this.appsecret,
                apptype = this.apptype
            };
        }
    }


}
