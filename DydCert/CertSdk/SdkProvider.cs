using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk
{
    public class SdkProvider
    {
        public OnlineCert oc = new OnlineCert();
        ISdk isdk = null;
        public SdkProvider(ISdk aisdk)
        {
            isdk = aisdk;
        }
        public Token Login(string username, string pwd)
        {
            Token t = oc.Login(username, pwd);
            Process_login(t);
            return t;
        }
        public Token Login(string username, string pwd, string appid, string appsecret)
        {
            Token t = oc.Login(username, pwd, appid, appsecret);
            Process_login(t);
            return t;
        }

        public void Process_login(Token t)
        {
            if (t != null)
            {
                var tok = isdk.Get(t.token);
                if (tok == null)
                {
                    isdk.Add(t);
                }
            }
        }

        public Token GetToken(string token)
        {
            var tokenobj = isdk.Get(token);
            if (tokenobj != null)
            {
                return tokenobj;
            }
            tokenobj = oc.ReqGetTokenInfo(token);
            if (token != null)
            {
                isdk.Add(tokenobj);
            }
            return tokenobj;
        }

        public void Logout(string token)
        { 
            
        }


    }
}
