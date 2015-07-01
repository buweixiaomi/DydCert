using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CertTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SdkNewTester sdknt = new SdkNewTester();
            sdknt.Start();
            Console.Read();

            InitToken toknsin = new InitToken();
            toknsin.Start();
            Console.Read();
            return;

            //TestDataReady tdr = new TestDataReady();
            //tdr.Start();
            //Console.Read();
            //return;
            NewSdkTest nst = new NewSdkTest();
            nst.Start();

            Console.Read();

            //CertSdk.newsdk.CertChain.ChainNode headnode = new CertSdk.newsdk.CertChain.ChainNode();
            //var tmpnode = headnode;
            //for (int i = 0; i < 100000000; i++)
            //{
            //    tmpnode.data = i;
            //    tmpnode.NextNode = new CertSdk.newsdk.CertChain.ChainNode();
            //    tmpnode = tmpnode.NextNode;
            //}
            //Stopwatch sw = new Stopwatch();
            //Console.WriteLine("开始查找。。。");
            //sw.Start();
            //int k = 99999999;
            //var currnode = headnode;
            //while (currnode!=null)
            //{
            //    if (currnode.data == k)
            //    {
            //        Console.WriteLine("searched");
            //        break;
            //    }
            //    currnode = currnode.NextNode;
            //}

            //sw.Stop();
            //Console.WriteLine("用时{0}ms", sw.ElapsedMilliseconds);
            //Console.Read();
            //for (int i = 0; i < 1000; i++)
            //{
            //    System.Threading.Thread t = new System.Threading.Thread(() =>
            //    {
            //        Random r = new Random(DateTime.Now.Millisecond);
            //        int aaa =5 ;//r.Next(0, 1000);
            //        while (true)
            //        {
            //            ReuquetTest.Request(aaa);
            //        }
            //    });
            //    t.Start();
            //}
        }
    }
}
