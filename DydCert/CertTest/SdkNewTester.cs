using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CertTest
{
    public class SdkNewTester
    {
        private List<string> willauthtokens = new List<string>();
        private List<string> willtestusers = new List<string>();
        private CertSdk.SdkProvider sp = new CertSdk.SdkProvider(CertSdk.newsdk.CertChain.GetInstance());
        public void Start()
        {

            StartMonitor();

            //准备 999000 token
            PrepareTokens(99000);

            //准备验证token
            GetSomeAuthToken(100);

            //测试用户
            GetTestUsers(100);

            Thread.Sleep(2000);
            //开始测试
            //开始大并测试
            Console.WriteLine("5分钟测试");
            TestAuth(100, 5);
            TestLogin(30, 5);
            Thread.Sleep(TimeSpan.FromMinutes(5));

            Thread.Sleep(2000);
            Console.WriteLine("5分钟测试");
            TestAuth(500, 5);
            TestLogin(100, 5);
            Thread.Sleep(TimeSpan.FromMinutes(5));

            Thread.Sleep(2000);
            Console.WriteLine("5分钟测试");
            TestAuth(2000, 5);
            TestLogin(100, 5);

            Thread.Sleep(TimeSpan.FromMinutes(5));
        }

        private int _PrepareTokens_count = 0;
        private void PrepareTokens(int allcount)
        {
            Console.WriteLine("正在准备内存token...");
            Thread mainthrad = new Thread(() =>
            {
                using (XXF.Db.DbConn qxdbconn = XXF.Db.DbConn.CreateConn(XXF.Db.DbType.SQLSERVER, "192.168.17.236", "dyd_new_qx", "sa", "Xx~!@#"))
                {
                    qxdbconn.Open();


                    // int allcount = 999000;// (int)dbconn.ExecuteScalar(sql_count, null);

                    int pagesize = 10000;

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < allcount; i = i + pagesize)
                    {
                        sb.Clear();
                        string sql = "select  * from (select row_number() over(order by id) as Rownum,* from usertoken ) A where A.Rownum between " + (i + 1) + " and " + (i + pagesize) + "";
                        DataTable tb = qxdbconn.SqlToDataTable(sql, null);
                        foreach (DataRow dr in tb.Rows)
                        {
                            sp.Add(new CertSdk.Token()
                            {
                                appid = dr["appid"].ToString(),
                                createtime = XXF.Db.LibConvert.ObjToDateTime(dr["createtime"]),
                                expires = XXF.Db.LibConvert.ObjToDateTime(dr["expires"]),
                                id = dr["id"].ToString(),
                                token = dr["token"].ToString(),
                                userid = dr["userid"].ToString(),
                                username = dr["username"].ToString()
                            });
                            _PrepareTokens_count++;
                        }
                    }
                }
            });
            Thread m = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(3000);
                    Console.WriteLine("当前准备token总数={0}", _PrepareTokens_count);
                    if (mainthrad.ThreadState == System.Threading.ThreadState.Stopped)
                        return;
                }
            });
            mainthrad.Start();
            m.Start();
            m.Join();

            Console.WriteLine("准备内存token完成！");
        }
        private int _GetSomeAuthToken_count = 0;
        public void GetSomeAuthToken(int count)
        {
            Console.WriteLine("正在准备验证用token...");
            Thread maint = new Thread(() =>
            {
                int pagesize = 50;
                using (XXF.Db.DbConn qxdbconn = XXF.Db.DbConn.CreateConn(XXF.Db.DbType.SQLSERVER, "192.168.17.236", "dyd_new_qx", "sa", "Xx~!@#"))
                {
                    qxdbconn.Open();

                    int allcount = (int)qxdbconn.ExecuteScalar("select count(1) from usertoken", null);
                    for (int i = 0; i < count; i = i + pagesize)
                    {
                        Random r = new Random();
                        int start_index = r.Next(1, allcount - pagesize);
                        string sql = "select top " + pagesize + " * from (select row_number() over(order by id) as Rownum,token from usertoken ) A where A.Rownum >=" + start_index + "";
                        DataTable tb = qxdbconn.SqlToDataTable(sql, null);
                        foreach (DataRow dr in tb.Rows)
                        {
                            willauthtokens.Add(dr["token"].ToString());
                            _GetSomeAuthToken_count++;
                        }
                    }
                }
            });
            Thread m = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(3000);
                    Console.WriteLine("当前准备验证用token总数={0}", _GetSomeAuthToken_count);
                    if (maint.ThreadState == System.Threading.ThreadState.Stopped)
                        return;
                }
            });
            maint.Start();
            m.Start();
            m.Join();

            Console.WriteLine("准备验证用token完成！");
        }

        private int _GetTestUsers_count = 0;
        public void GetTestUsers(int count)
        {
            Console.WriteLine("正在准备登录用户...");
            Thread maint = new Thread(() =>
            {
                int pagesize = 50;
                using (XXF.Db.DbConn qxdbconn = XXF.Db.DbConn.CreateConn(XXF.Db.DbType.SQLSERVER, "192.168.17.236", "cert_test_main", "sa", "Xx~!@#"))
                {
                    qxdbconn.Open();
                    int allcount = (int)qxdbconn.ExecuteScalar("select count(1) from tb_customer", null);
                    for (int i = 0; i < count; i = i + pagesize)
                    {
                        Random r = new Random();
                        int start_index = r.Next(1, allcount - pagesize);
                        string sql = "select top " + pagesize + " * from (select row_number() over(order by f_id) as Rownum,* from tb_customer ) A where A.Rownum >=" + start_index + "";
                        DataTable tb = qxdbconn.SqlToDataTable(sql, null);
                        foreach (DataRow dr in tb.Rows)
                        {
                            willtestusers.Add(dr["f_yhzh"].ToString());
                            _GetTestUsers_count++;
                        }
                    }
                }
            });
            Thread m = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(3000);
                    Console.WriteLine("当前准备测试用户总数={0}", _GetTestUsers_count);
                    if (maint.ThreadState == System.Threading.ThreadState.Stopped)
                        return;
                }
            });
            maint.Start();
            m.Start();
            m.Join();

            Console.WriteLine("准备登录用户完成！");
        }


        private bool TestLogin_cancel_token = false;
        public void TestLogin(int process, int mins)
        {
            Console.WriteLine("正在测试登录，线程数:{0} 测试时间:{1}", process, mins);
            for (int i = 0; i < process; i++)
            {
                Thread t = new Thread(() =>
                {
                    while (true)
                    {
                        Random r = new Random(DateTime.Now.Millisecond);
                        string userid = willtestusers[r.Next(0, willtestusers.Count)];
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var token = sp.Login(userid, "123456");
                        sw.Stop();
                        Console.WriteLine("\t[登录]token:{0,36} time:{1,10}ms", token == null ? "" : token.token, sw.ElapsedMilliseconds);
                        if (TestLogin_cancel_token)
                            return;
                    }
                });
                t.Start();
            }
            Thread tm = new Thread(() =>
            {
                Thread.Sleep(TimeSpan.FromMinutes(mins));
                TestLogin_cancel_token = true;
                Console.WriteLine("测试登录完成");
            });
            tm.Start();
        }

        private bool TestAuth_cancel_Token = false;
        public void TestAuth(int process, int mins)
        {
            Console.WriteLine("正在测试验证，线程数:{0} 测试时间:{1}", process, mins);
            for (int i = 0; i < process; i++)
            {
                Thread t = new Thread(() =>
                {
                    while (true)
                    {
                        Random r = new Random(DateTime.Now.Millisecond);
                        string strtoken = willauthtokens[r.Next(0, willauthtokens.Count)];
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var token = sp.GetToken(strtoken);
                        sw.Stop();
                        Console.WriteLine("[验证]token:{0,36} time:{1,10}ms", token == null ? "" : token.token, sw.ElapsedMilliseconds);
                        if (TestAuth_cancel_Token)
                            return;
                    }
                });
                t.Start();
            }
            Thread tm = new Thread(() =>
            {
                Thread.Sleep(TimeSpan.FromMinutes(mins));
                TestAuth_cancel_Token = true;
                Console.WriteLine("测试验证完成！");
            });
            tm.Start();
        }


        public Thread StartMonitor()
        {

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    var a = ApiInvokeMap.MapCore.GetInstance().GetMap();
                    //a.Add(new ApiInvokeMap.MapItem() { url = "TokensCount", lastcount = sp.GetAll().Count });
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    foreach (var b in a)
                    {
                        sb.AppendFormat("{0,20}", b.url);
                    }
                    sb.AppendLine();
                    foreach (var b in a)
                    {
                        sb.AppendFormat("{0,20}", b.lastcount);
                    }
                    System.IO.File.AppendAllText("D:\\SdkNewTesterlog.txt", sb.ToString());
                    Thread.Sleep(ApiInvokeMap.MapCore.GetInstance().GetWrapTime());
                }

            });
            t.Start();
            return t;
        }

    }
}
