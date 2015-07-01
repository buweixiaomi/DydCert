using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace CertTest
{
    class InitToken
    {

        private void Init(object obj)
        {

            int[] para = obj == null ? new int[] { 1, 1 } : (int[])obj;
            using (XXF.Db.DbConn dbconn = XXF.Db.DbConn.CreateConn(XXF.Db.DbType.SQLSERVER, "192.168.17.236", "cert_test_main", "sa", "Xx~!@#"))
            using (XXF.Db.DbConn qxdbconn = XXF.Db.DbConn.CreateConn(XXF.Db.DbType.SQLSERVER, "192.168.17.236", "dyd_new_qx", "sa", "Xx~!@#"))
            {
                dbconn.Open();
                qxdbconn.Open();
                int allcount = 1200000;// (int)dbconn.ExecuteScalar(sql_count, null);

                int threadsizecount = (int)Math.Ceiling(allcount / (double)para[1]);

                int pagesize = 200;

                int start = (para[0]) * threadsizecount;
                int end = (para[0] + 1) * threadsizecount;

                StringBuilder sb = new StringBuilder();
                for (int i = start; i < end; i = i + pagesize)
                {
                    sb.Clear();
                    string sql = "select top " + pagesize + " * from (select row_number() over(order by f_id) as Rownum,* from tb_customer ) A where A.Rownum between " + (i + 1) + " and " + (end) + "";
                    DataTable tb = dbconn.SqlToDataTable(sql, null);
                    foreach (DataRow dr in tb.Rows)
                    {
                        count_me[para[0]]++;
                        string pwd = dr["f_dlmm"].ToString();
                        string yhzh = dr["f_yhzh"].ToString();
                        string id = dr["f_id"].ToString();
                        string yhxm = dr["f_yhxm"].ToString();
                        string token = Guid.NewGuid().ToString().Replace("-", "");
                        sb.AppendFormat("insert into usertoken (token,userid,id,username,appid,createtime,expires) " +
                                                 "  values('{0}','{1}','{2}','{3}','Customer','2015-07-01','2015-08-31');\r\n", token, yhzh, id, yhxm);
                    }
                    qxdbconn.ExecuteSql(sb.ToString(), null);

                }
            }

        }



        public void Start()
        {
            MultyThreadInit(12);
            System.Threading.Thread t = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("当前总数={0}", count_me.Sum());
                    Thread.Sleep(3000);
                }
            });
            t.Start();
        }


        public int[] count_me = null;
        private object ok_init = new object();
        private void MultyThreadInit(int thread_count)
        {
            count_me = new int[thread_count];
            for (int k = 0; k < thread_count; k++)
            {
                if (k == 11)
                {
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Init));
                    t.Start(new int[] { k, thread_count });
                }
            }
        }



    }
}
