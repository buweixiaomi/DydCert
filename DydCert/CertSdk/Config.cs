using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk
{
    public class Config
    {
        //private static string _HOSTURL = "http://192.168.17.200:3388";
        private static string HOSTURL
        {
            get
            {
                string t = XXF.Db.DbConfig.GetConfig("CertCenterUrl");
                if (string.IsNullOrEmpty(t))
                    t = "http://192.168.17.208:3388";
                if (t.EndsWith("/"))
                    t = t.Remove(t.Length - 1, 1);
                return t;
            }
        }

        public static string TOKEN_URL = HOSTURL + "/certapi/usertoken/get";
        public static string AUTH_URL = HOSTURL + "/certapi/usertoken/auth";
        public static string APILIST_URL = HOSTURL + "/certapi/usertoken/getapilist";
        public static string TOKENINFO_URL = HOSTURL + "/certapi/usertoken/gettokeninfo";
        public static string REFRESH_TOKEN_URL = HOSTURL + "/certapi/usertoken/RefreshToken";
        public static string LOGOUT_URL = HOSTURL + "/certapi/usertoken/logout";
    }
}
