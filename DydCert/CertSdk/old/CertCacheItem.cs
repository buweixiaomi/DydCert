using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertSdk.old.CertCenter
{
    public class CertCacheItem : ICloneable
    {
        public AuthToken Token { get; set; }
        public List<api> apis { get; set; }

        public object Clone()
        {
            CertCacheItem item = new CertCacheItem()
            {
                Token = this.Token.Clone() as AuthToken,
                apis = this.apis.CloneList() as List<api>
            };
            return item;
            
        }



    }
}
