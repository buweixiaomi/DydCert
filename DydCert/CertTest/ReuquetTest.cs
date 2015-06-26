using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertTest
{
    public class ReuquetTest
    {
        static string[] urls = new string[] {
        "/certapi/appsevice/getappsecret",

        "/certapi/usertoken/get",
        "/certapi/usertoken/auth",
        "/certapi/usertoken/getapilist",
        "/certapi/usertoken/gettokeninfo",
        "/certapi/usertoken/refreshtoken",
        "/certapi/usertoken/logout",

        
        "/certapi/shoptoken/get",
        "/certapi/shoptoken/auth",
        "/certapi/shoptoken/getapilist",
        "/certapi/shoptoken/gettokeninfo",
        "/certapi/shoptoken/refreshtoken",
        "/certapi/shoptoken/logout",

        
        "/certapi/managetoken/get",
        "/certapi/managetoken/auth",
        "/certapi/managetoken/getapilist",
        "/certapi/managetoken/gettokeninfo",
        "/certapi/managetoken/refreshtoken",
        "/certapi/managetoken/logout",
        
        
        };
        public static void Request(int i)
        {
            string url = urls[i % urls.Length];
            XXF.Api.HttpServer.InvokeApi(url, null);
        }
    }
}
