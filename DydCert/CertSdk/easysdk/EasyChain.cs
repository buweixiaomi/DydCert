using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk.easysdk
{
    public class EasyChain : ISdk
    {

        private List<EasyChainItem> tokens = new List<EasyChainItem>();
        private int _maxlength = 50000;//000;
        private int _currlength = 0;
        private System.Threading.Thread maintance_thread = null;
        private static EasyChain Instance = new EasyChain();
        static EasyChain() { }
        private EasyChain()
        {
            maintance_thread = new System.Threading.Thread(MaintanceThreadAction);
            maintance_thread.Start();
        }

        public void MaintanceThreadAction()
        {
            try
            {
                while (true)
                {
                    if ((_currlength / (double)_maxlength) > Convert.ToDouble(GetConfig("SortPercent", "0.3")))
                    {
                        Sort();
                        if ((_currlength / (double)_maxlength) > Convert.ToDouble(GetConfig("SortPercent", "0.8")))
                        {
                            BatchDelete();
                        }
                    }
                    System.Threading.Thread.Sleep(TimeSpan.FromMinutes(Convert.ToDouble(GetConfig("MainSleepMins", "10"))));
                }
            }
            catch (Exception ex) { }
        }

        public void Add(Token t)
        {
            lock (tokens)
            {
                if (_currlength == _maxlength)
                    Delete();


                tokens.Add(new EasyChainItem()
                {
                    accesscount = 0,
                    used = false,
                    token = t
                });
                _currlength++;
            }
        }


        public void Delete()
        {
            lock (tokens)
            {
                if (tokens.Count > 0)
                {
                    tokens.RemoveAt(tokens.Count - 1);
                    _currlength--;
                }
            }
        }

        public void BatchDelete()
        {
            lock (tokens)
            {
                int c_c = tokens.Count(x => x.used == false);
                tokens.RemoveAll(x => x.used == false);
                foreach (var a in tokens)
                    a.used = false;
                _currlength -= c_c;
            }
        }


        public void Sort()
        {
            lock (tokens)
            {
                tokens.Sort((x, y) => { return x.accesscount - y.accesscount; });
                foreach (var a in tokens)
                    a.accesscount = 0;
            }
        }

        public Token Get(string token)
        {
            lock (tokens)
            {
                foreach (var a in tokens)
                {
                    if (a.token.token == token)
                    {
                        lock (a.token)
                        {
                            a.accesscount++;
                            a.used = true;
                        }
                        return a.token;
                    }
                }
            }
            return null;
        }
        private string GetConfig(string name, string def)
        {
            string v = System.Configuration.ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(v))
                return def;
            return v;
        }
        private class EasyChainItem
        {
            public int accesscount { get; set; }
            public Token token { get; set; }
            public bool used { get; set; }
        }

        public List<Token> GetTokens()
        {
            List<Token> ttt = new List<Token>();
            lock (tokens)
            {
                int maxcount = 100;
                foreach (var t in tokens)
                {
                    ttt.Add(t.token);
                    maxcount--;
                    if (maxcount < 0)
                        break;
                }
            }
            return ttt;
        }

        public static ISdk GetInstance()
        {
            return Instance;
        }


        public int GetLength()
        {
            return _currlength;
        }


        public void WriteTopToFile(int topcount)
        {
            StringBuilder sb = new StringBuilder();
            lock (tokens)
            {
                for (int i = 0; i < topcount; i++)
                {
                    sb.AppendFormat("{0,6} {1,4} {2,40} {3,15}\r\n ", tokens[i].accesscount, tokens[i].used, tokens[i].token.token, tokens[i].token.userid);
                }
            }
            System.IO.File.AppendAllText("D:\\" + this.GetType().FullName + ".top.txt", sb.ToString());
        }
    }
}
