using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertSdk.old.CertCenter
{
    public class CertApiConfig
    {
        //private static string _HOSTURL = "http://192.168.17.200:3388";
        private static string HOSTURL
        {
            get
            {
              string t =  XXF.Db.DbConfig.GetConfig("CertCenterUrl");
              if (string.IsNullOrEmpty(t))
                  t = "http://192.168.17.208:3388";
              if (t.EndsWith("/"))
                  t = t.Remove(t.Length - 1, 1);
              return t;
            }
        }
        private ServiceCertType _certtype;
        private Dictionary<ServiceCertType, string> certypetocon;

        private static string TOKEN_URL = HOSTURL + "/certapi/{0}/get";


        private static string AUTH_URL = HOSTURL + "/certapi/{0}/auth";


        private static string APILIST_URL = HOSTURL + "/certapi/{0}/getapilist";

        private static string TOKENINFO_URL = HOSTURL + "/certapi/{0}/gettokeninfo";

        private static string REFRESH_TOKEN_URL = HOSTURL + "/certapi/{0}/RefreshToken";


        private static string LOGOUT_URL = HOSTURL + "/certapi/{0}/logout";

        public CertApiConfig(ServiceCertType certtype)
        {
            certypetocon = new Dictionary<ServiceCertType, string>();
            certypetocon.Add(ServiceCertType.manage, "managetoken");
            certypetocon.Add(ServiceCertType.shop, "shoptoken");
            certypetocon.Add(ServiceCertType.user, "usertoken");
            _certtype = certtype;
        }


        /// <summary>
        /// 登录得到token 的url
        /// </summary>
        public string tokenurl
        {
            get
            {
                return string.Format(TOKEN_URL, certypetocon[_certtype]);
            }
        }

        public string appsecreturl
        {
             get
            {
                return HOSTURL + "/certapi/appsevice/GetAppSecret";
            }
        }
        /// <summary>
        /// 权限验证的url
        /// </summary>
        public string authurl
        {
            get
            {
                return string.Format(AUTH_URL, certypetocon[_certtype]);
            }
        }

        public string logouturl
        {
            get
            {
                return string.Format(LOGOUT_URL, certypetocon[_certtype]);
            }
        }

        /// <summary>
        /// 得到有权访问的api列表的url
        /// </summary>
        public string apilisturl
        {
            get
            {
                return string.Format(APILIST_URL, certypetocon[_certtype]);
            }
        }

        /// <summary>
        /// token 得到 Token信息 的 url
        /// </summary>
        public string tokeninfourl
        {
            get
            {
                return string.Format(TOKENINFO_URL, certypetocon[_certtype]);
            }
        }

        public string refreshtokenurl
        {
            get
            {
                return string.Format(REFRESH_TOKEN_URL, certypetocon[_certtype]);
            }
        }
    }
}
