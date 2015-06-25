using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertSdk.old.CertCenter
{
    public class CertCache
    {
        private List<CertCacheItem> tempmanagecache;

        private ServiceCertType certtype { get; set; }

        public CertCache(ServiceCertType certtype)
        {

            this.certtype = certtype;
            System.Runtime.Caching.MemoryCache mcache = System.Runtime.Caching.MemoryCache.Default;
            List<CertCacheItem> _tempmanagecache = mcache.Get(cachetypename) as List<CertCacheItem>;
            if (_tempmanagecache == null)
            {
                _tempmanagecache = new List<CertCacheItem>();
                bool r = mcache.Add(cachetypename, _tempmanagecache, new DateTimeOffset(DateTime.Now.AddMinutes(expiresminutes)));
                if (!r)
                {
                    throw new Exception("初始化缓存失败。");
                }
            }
            tempmanagecache = _tempmanagecache.CloneList();

        }

        private void SetCertCahe()
        {
            System.Runtime.Caching.MemoryCache mcache = System.Runtime.Caching.MemoryCache.Default;
            if (tempmanagecache == null)
                tempmanagecache = new List<CertCacheItem>();
            if (mcache.Get(cachetypename) != null)
            {
                mcache.Remove(cachetypename);
            }

            bool r = mcache.Add(cachetypename, tempmanagecache.CloneList(), new DateTimeOffset(DateTime.Now.AddMinutes(expiresminutes)));
            if (!r)
            {
                mcache = null;
            }
        }

        private string cachetypename
        {

            get
            {
                const string basekey = "CertCache.Cert.";
                switch (certtype)
                {
                    case ServiceCertType.user:
                        return basekey + "usercache";
                    case ServiceCertType.shop:
                        return basekey + "shopcache";
                    case ServiceCertType.manage:
                    default:
                        return basekey + "managecache";
                }
            }
        }

        public int expiresminutes
        {
            get
            {
                switch (certtype)
                {
                    case ServiceCertType.user:
                        return 1 * 24 * 60;
                    case ServiceCertType.shop:
                        return 7 * 24 * 60;
                    case ServiceCertType.manage:
                    default:
                        return 1 * 24 * 60;
                }
            }
        }

        public void AddOrUpdateToken(AuthToken Token)
        {
            CertCacheItem t_tokenitem = null;
            Token.lastauth = DateTime.Now;//上次请求certcennt的时间。
            t_tokenitem = tempmanagecache.FirstOrDefault(x => x.Token.token == Token.token);
            if (t_tokenitem == null)
                tempmanagecache.Add(new CertCacheItem() { Token = Token });
            else
            {
                t_tokenitem.Token = Token;
            }
            SetCertCahe();
        }


        public bool HasToken(string token)
        {
            if (GetCacheToken(token) == null)
                return false;
            return true;
        }

        public CertCacheItem GetCacheToken(string token)
        {
            CertCacheItem t_tokenitem = null;
            try
            {
                t_tokenitem = tempmanagecache.FirstOrDefault(x => x.Token.token == token);
                if (t_tokenitem == null)
                    return null;
            }
            catch
            {

            }
            return t_tokenitem;
        }

        public bool SetCacheApis(string token, List<api> apis)
        {
            CertCacheItem item = GetCacheToken(token);
            if (item == null)
                return false;
            item.apis = apis;
            SetCertCahe();
            return true;
        }

        public void DeleteToken(string token)
        {
            CertCacheItem item = GetCacheToken(token);
            if (item != null)
            {
                tempmanagecache.Remove(item);
                SetCertCahe();
            }
        }

    }

    /// <summary>
    /// List特定类型的扩展。需要配合内部类型的clone方法
    /// </summary>
    public static class CopyExtention
    {
        /// <summary>
        /// clonelist 需要配合内部类型的clone方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<CertCacheItem> CloneList(this List<CertCacheItem> obj)
        {
            if (obj == null)
                return null;
            List<CertCacheItem> newobj = new List<CertCacheItem>();
            foreach (CertCacheItem item in obj)
            {
                newobj.Add(item.Clone() as CertCacheItem);
            }
            return newobj;
        }
        /// <summary>
        /// clonelist 需要配合内部类型的clone方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<api> CloneList(this List<api> obj)
        {
            if (obj == null)
                return null;
            List<api> newobj = new List<api>();
            foreach (api item in obj)
            {
                newobj.Add(item.Clone() as api);
            }
            return newobj;
        }
        public static List<AppModel> CloneList(this List<AppModel> obj)
        {
            if (obj == null)
                return null;
            List<AppModel> newobj = new List<AppModel>();
            foreach (AppModel item in obj)
            {
                newobj.Add(item.Clone() as AppModel);
            }
            return newobj;
        }

    }


}
