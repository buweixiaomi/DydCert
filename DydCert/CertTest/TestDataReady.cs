using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CertTest
{
    public class TestDataReady
    {
        private string pwd = string.Empty;
        private long start_userid = 158324531+ 1;
        private readonly int count = 1500000;
        public int currcount { get; private set; }
        private Stopwatch sw = new Stopwatch();
        public TestDataReady()
        {
            pwd = XXF.Db.LibCrypto.EnDES("123456");
        }

        public void Start()
        {
            Thread th = new Thread(Do);
            Thread t2 = new Thread(() =>
            {
                while (true)
                {
                    double ps = (currcount) / (double)count;
                    int used_minis = sw.Elapsed.Minutes;
                    int last_minits = (int)(used_minis / ps);
                    Console.WriteLine("currcount={0,13} percent={1,5}% usedmins={2,5} lastmins={3,5}", currcount, (ps * 100).ToString("0.00"), used_minis, last_minits);
                    Thread.Sleep(3000);
                }
            });
            th.Start();
            t2.Start();
            sw.Start();
        }


        private void Do()
        {
            using (XXF.Db.DbConn pubconn = XXF.Db.DbConn.CreateConn(XXF.Db.DbType.SQLSERVER, "192.168.17.236", "cert_test_main", "sa", "Xx~!@#"))
            {
                pubconn.Open();
                long curr_uid = start_userid;
                StringBuilder sb = new StringBuilder();
                while (currcount < count)
                {
                    sb.Clear();
                    for (int i = 0; i < 1000; i++)
                    {
                        sb.AppendFormat("insert into tb_customer(f_yhzh,f_yhxm,f_dlmm,f_sfdj) values('{0}','{1}','{2}',0);\r\n", curr_uid, "yhm_" + curr_uid, pwd);

                        curr_uid++;
                        currcount++;
                    }
                    pubconn.ExecuteSql(sb.ToString(), null);
                }
            }
        }

    }
}
