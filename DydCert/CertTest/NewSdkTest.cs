using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CertTest
{
    public class NewSdkTest
    {
       private CertSdk.SdkProvider sp = new CertSdk.SdkProvider(CertSdk.newsdk.CertChain.GetInstance());
         //private CertSdk.SdkProvider sp = new CertSdk.SdkProvider(CertSdk.easysdk.EasyChain.GetInstance());
        List<CertSdk.Token> tokens = new List<CertSdk.Token>();
        public void Start()
        {
            var m_thread = StartMonitor();
            MultyThreadInit(100);
            Thread.Sleep(1000);
            Monitor.Enter(ok_init);
            // m_thread.Abort();
            PrintAllToken();
            TestAuth(5);
        }

        #region Step_one
        private void Init(object obj)
        {

            int[] para = obj == null ? new int[] { 1, 1 } : (int[])obj;
            using (XXF.Db.DbConn dbconn = XXF.Db.DbConn.CreateConn(XXF.Db.DbType.SQLSERVER, "192.168.17.205", "dyd_new_main", "sa", "Xx~!@#"))
            {
                dbconn.Open();
                string sql_count = "select count(1) from tb_customer";
                int allcount = 40000;// (int)dbconn.ExecuteScalar(sql_count, null);

                int threadsizecount = (int)Math.Ceiling(allcount / (double)para[1]);

                int pagesize = 1000;

                int start = (para[0]) * threadsizecount;
                int end = (para[0] + 1) * threadsizecount;

                for (int i = start; i < end; i = i + pagesize)
                {
                    string sql = "select top " + pagesize + " * from (select row_number() over(order by f_id) as Rownum,* from tb_customer ) A where A.Rownum between " + (i + 1) + " and " + (end) + "";
                    DataTable tb = dbconn.SqlToDataTable(sql, null);
                    foreach (DataRow dr in tb.Rows)
                    {
                        count_me[para[0]]++;
                        string pwd = dr["f_dlmm"].ToString();
                        try
                        {
                            pwd = XXF.Db.LibCrypto.DeDES(pwd);
                        }
                        catch (Exception ex)
                        {

                        }
                        var token = sp.Login(dr["f_yhzh"].ToString(), pwd);
                        if (token != null)
                        {
                            lock (tokens)
                                tokens.Add(token);
                            Console.WriteLine(token.token);
                        }
                        else
                            Console.WriteLine("error");
                    }
                }
            }

        }

        public int[] count_me = null;
        public object ok_init = new object();
        private void MultyThreadInit(int thread_count)
        {

            count_me = new int[thread_count];
            for (int k = 0; k < thread_count; k++)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Init));
                t.Start(new int[] { k, thread_count });
            }
            System.Threading.Thread r = new System.Threading.Thread((x) =>
            {
                lock (ok_init)
                {
                    int last = 0;
                    int last_equal_times = 0;
                    while (true)
                    {
                        Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
                        var t = count_me.Sum();
                        if (last == t)
                        {
                            last_equal_times++;
                            if (last_equal_times == 3)
                                break;
                        }
                        else
                        {
                            last = t;
                        }
                        Console.WriteLine(t);
                        Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            });
            r.Start();
        }

        #endregion

        #region Step_two
        public void PrintAllToken()
        {
           var memorytokens = sp.GetAll();
           if (memorytokens != null)
            {

                foreach (var a in memorytokens)
                {
                    Console.WriteLine(a.token);
                }
            }
            Console.WriteLine("Token 得到完成，");
        }
        #endregion

        public Thread StartMonitor()
        {

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    var a = ApiInvokeMap.MapCore.GetInstance().GetMap();
                    a.Add(new ApiInvokeMap.MapItem() { url = "TokensCount", lastcount = sp.GetAll().Count });
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (var b in a)
                    {
                        sb.AppendFormat("{0,12}", b.url);
                    }
                    sb.AppendLine();
                    foreach (var b in a)
                    {
                        sb.AppendFormat("{0,12}", b.lastcount);
                    }
                    System.IO.File.AppendAllText("D:\\logsss.txt", sb.ToString());
                    Thread.Sleep(ApiInvokeMap.MapCore.GetInstance().GetWrapTime());
                }

            });
            t.Start();
            return t;
        }

        public void TestAuth(int threadcount)
        {
            for (int i = 0; i < threadcount; i++)
            {
                Thread t = new Thread(() =>
                {
                    while (true)
                    {
                        Random r = new Random(DateTime.Now.Millisecond);
                        int index = r.Next(0, tokens.Count);
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var getnewtoken = sp.GetToken(tokens[index].token);
                        sw.Stop();
                        Console.WriteLine("Token = {0,35} Time={1,16}", getnewtoken == null ? "" : getnewtoken.token, sw.ElapsedMilliseconds);
                    }
                });
                t.IsBackground = false;
                t.Start();
            }
        }
    }
}
