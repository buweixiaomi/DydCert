using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertTest
{
    public class OldSdkTest
    {
        public void Test()
        {
            CertSdk.old.CertCenter.CertCenterProvider ccp = new CertSdk.old.CertCenter.CertCenterProvider(CertSdk.old.CertCenter.ServiceCertType.user, false, false);
            ccp.Login("", "123456", "Customer", "1234");
        }
    }
}
