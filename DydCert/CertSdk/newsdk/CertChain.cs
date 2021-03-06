﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk.newsdk
{
    public class CertChain : ISdk
    {
        private int _maxlength = 1000000;//100万
        private int _currlength = 0;
        private ChainNode _headNode = null;
        private ChainNode _tailNode = null;

        // private object init_add_look_obj = new object();
        //   private object init_sort_look_obj = new object();
        private object add_look_obj = new object();
        private static CertChain Instance = new CertChain();
        private System.Threading.Thread maintance_thread = null;
        static CertChain() { }
        private CertChain()
        {
            maintance_thread = new System.Threading.Thread(MaintanceThreadAction);
            maintance_thread.Start();
        }
        public static CertChain GetInstance()
        {
            return Instance;
        }
        private void MaintanceThreadAction()
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
                            //  BatchDelete();
                        }
                    }
                    System.Threading.Thread.Sleep(TimeSpan.FromMinutes(Convert.ToDouble(GetConfig("MainSleepMins", "1"))));
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void BatchDelete()
        {
            ApiInvokeMap.MapCore.GetInstance().Increase("BatchDelete");
            if (_tailNode != null)
            {
                ChainNode result_node = null;
                var tn = _tailNode;
                int r_count = 0;
                while (tn != null && !tn.used)
                {
                    result_node = tn;
                    tn = tn.preNode;
                    r_count++;
                }
                if (result_node != null)
                {
                    if (result_node.preNode != null)
                    {
                        lock (result_node.preNode)
                        {
                            result_node.preNode.nextNode = null;
                        }
                        _tailNode = result_node.preNode;
                    }
                    else
                    {
                        _tailNode = null;
                        _headNode = null;
                    }
                    lock (result_node)
                    {
                        result_node.abort = true;
                        _currlength -= r_count;
                    }
                }
            }
        }

        private void Sort()
        {
            ApiInvokeMap.MapCore.GetInstance().Increase("Sort");
            lock (add_look_obj)
            {
                if (_currlength == 0)
                    return;
                List<ChainNode> tosortlist = new List<ChainNode>(_currlength);
                var tempnode = _headNode;
                //var curr_first = _headNode;
                while (tempnode != null)
                {
                    tosortlist.Add(ChainNode.ShallowClone(tempnode));

                    tempnode = tempnode.nextNode;
                }

                tosortlist.Sort((x, y) => { return y.accessCount - x.accessCount; });

                //relink
                tosortlist[0].preNode = null;
                tosortlist[tosortlist.Count - 1].nextNode = null;
                tosortlist[0].accessCount = 0;
                tosortlist[tosortlist.Count - 1].accessCount = 0;
                if (tosortlist.Count > 1)
                {
                    tosortlist[0].nextNode = tosortlist[1];
                    tosortlist[tosortlist.Count - 1].preNode = tosortlist[tosortlist.Count - 2];
                }
                else
                {
                    tosortlist[0].nextNode = null;
                    tosortlist[tosortlist.Count - 1].preNode = null;
                }
                for (int i = 1; i < tosortlist.Count - 1; i++)
                {
                    tosortlist[i].preNode = tosortlist[i - 1];
                    tosortlist[i].nextNode = tosortlist[i + 1];
                    tosortlist[i].accessCount = 0;
                }

                _tailNode = tosortlist[tosortlist.Count - 1];
                _headNode = tosortlist[0];

            }
        }

        private void Delete()
        {
            ApiInvokeMap.MapCore.GetInstance().Increase("Delete");

            lock (add_look_obj)
            {

                if (_currlength == 0)
                    return;

                if (_tailNode.preNode != null)
                {
                    _tailNode.preNode.nextNode = null;
                    _tailNode = _tailNode.preNode;
                    _currlength--;
                }
                else
                {
                    _currlength = 0;
                    _headNode = null;
                    _tailNode = null;
                }
            }

        }

        public void Add(Token t)
        {
            lock (add_look_obj)
            {
                ApiInvokeMap.MapCore.GetInstance().Increase("Add");
                if (_currlength == 0)
                {
                    if (InitAdd(t))
                    {
                        return;
                    }
                }
                if (_currlength == _maxlength)
                {
                    Delete();
                }
                ChainNode nodeitem = new ChainNode(t);
                _headNode.preNode = nodeitem;
                nodeitem.nextNode = _headNode;

                _headNode = nodeitem;
                _currlength++;
            }

        }

        public Token Get(string token)
        {
            ApiInvokeMap.MapCore.GetInstance().Increase("Get");
            var search_curr = _headNode;
            while (search_curr != null)
            {
                if (search_curr.token.token == token)
                {
                    lock (search_curr)
                    {
                        if (search_curr.used == false)
                            search_curr.used = true;
                        search_curr.accessCount++;
                    }
                    return search_curr.token;
                }
                search_curr = search_curr.nextNode;
            }
            return null;
        }

        private bool InitAdd(Token t)
        {
            ApiInvokeMap.MapCore.GetInstance().Increase("InitAdd");
            lock (add_look_obj)
            {
                if (_currlength != 0)
                    return false;
                ChainNode nodeitem = new ChainNode(t);

                _headNode = nodeitem;
                _tailNode = nodeitem;
                _currlength = 1;
                return true;
            }
        }

        private string GetConfig(string name, string def)
        {
            ApiInvokeMap.MapCore.GetInstance().Increase("GetConfig");
            string v = System.Configuration.ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(v))
                return def;
            return v;
        }

        private class ChainNode
        {
            public static ChainNode ShallowClone(ChainNode tnode)
            {
                var newnode = new ChainNode(tnode.token)
                {
                    accessCount = tnode.accessCount,
                    nextNode = tnode.nextNode,
                    preNode = tnode.preNode,
                    used = tnode.used
                };
                return newnode;
            }

            public ChainNode(Token t)
            {
                token = t;
            }
            public bool abort { get; set; }
            public ChainNode nextNode { get; set; }
            public ChainNode preNode { get; set; }
            public bool used { get; set; }
            public int accessCount { get; set; }
            public Token token { get; set; }
        }


        public List<Token> GetTokens()
        {
            ApiInvokeMap.MapCore.GetInstance().Increase("GetTokens");
            List<Token> ttt = new List<Token>();
            var t = _tailNode;
            while (t != null)
            {
                ttt.Add(t.token);
                t = t.preNode;
            }
            return ttt;
        }


        public int GetLength()
        {
            return _currlength;
        }


        public void WriteTopToFile(int topcount)
        {
            var n = _headNode;
            int c = 0;
            StringBuilder sb = new StringBuilder();
            while (n != null)
            {
                sb.AppendFormat("{0,6} {1,4} {2,40} {3,15}\r\n ", n.accessCount, n.used, n.token.token, n.token.userid);
                n = n.nextNode;
                c++;
                if (c >= topcount)
                    break;
            }
            System.IO.File.AppendAllText("D:\\" + this.GetType().FullName + ".top.txt", sb.ToString());
        }
    }
}
