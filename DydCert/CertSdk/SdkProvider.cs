using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void Process_login(Token t)
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
            ApiInvokeMap.MapCore.GetInstance().Increase("GetToken");
            Token tokenobj = isdk.Get(token);
            if (tokenobj != null)
            {
                tokenobj.id = "[Local]";
                return tokenobj;
            }
            ApiInvokeMap.MapCore.GetInstance().Increase("ReqGetTokenInfo");
            tokenobj = oc.ReqGetTokenInfo(token);
            if (tokenobj != null)
            {
                tokenobj.id = "[Online]";
                isdk.Add(tokenobj);
            }
            return tokenobj;
        }

        public void Logout(string token)
        {

        }

        public List<Token> GetAll()
        {
            return isdk.GetTokens();
        }

        public void Add(Token token)
        {
            isdk.Add(token);
        }

        public int GetLength()
        {
            return isdk.GetLength();
        }

        public void WriteTopToFile(int c)
        {
            isdk.WriteTopToFile(c);
        }
    }
}
