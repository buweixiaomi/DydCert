using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertTest
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                System.Threading.Thread t = new System.Threading.Thread(() =>
                {
                    Random r = new Random(DateTime.Now.Millisecond);
                    int aaa =5 ;//r.Next(0, 1000);
                    while (true)
                    {
                        ReuquetTest.Request(aaa);
                    }
                });
                t.Start();
            }
        }
    }
}
