using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XXF.Db;

namespace CertCenter.Models
{
    public class CertCenterLogDal
    {
        public readonly static CertCenterLogDal Instance = new CertCenterLogDal();

        public virtual bool Add(DbConn PubConn, DbModels.certcenterlog model)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					new ProcedureParameter("@url",    model.url),
					//
					new ProcedureParameter("@reqdata",    model.reqdata),
					//
					new ProcedureParameter("@userid",    model.userid),
					new ProcedureParameter("@username",    model.username),
					//
					new ProcedureParameter("@reqtime",    model.reqtime),
					//
					new ProcedureParameter("@ip",    model.ip),
					//
					new ProcedureParameter("@opecontent",    model.opecontent)   
                };
            int rev = PubConn.ExecuteSql(@"insert into certcenterlog(url,reqdata,userid,username,reqtime,ip,opecontent)
										   values(@url,@reqdata,@userid,@username,@reqtime,@ip,@opecontent)", Par);
            return rev == 1;
        }

        public virtual bool Edit(DbConn PubConn, DbModels.certcenterlog model)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
					new ProcedureParameter("@url",    model.url),
					//
					new ProcedureParameter("@reqdata",    model.reqdata),
					//
					new ProcedureParameter("@userid",    model.userid),
					new ProcedureParameter("@username",    model.username),
					//
					new ProcedureParameter("@reqtime",    model.reqtime),
					//
					new ProcedureParameter("@ip",    model.ip),
					//
					new ProcedureParameter("@opecontent",    model.opecontent)
            };
            Par.Add(new ProcedureParameter("@id", model.id));

            int rev = PubConn.ExecuteSql("update certcenterlog set url=@url,reqdata=@reqdata,userid=@userid,username =@username,reqtime=@reqtime,ip=@ip,opecontent=@opecontent where id=@id", Par);
            return rev == 1;

        }

        public virtual bool Delete(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", id));

            string Sql = "delete from certcenterlog where id=@id";
            int rev = PubConn.ExecuteSql(Sql, Par);
            if (rev == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public virtual DbModels.certcenterlog Get(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", ProcParType.Int32, 4, id));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from certcenterlog s where s.id=@id");
            int rev = PubConn.ExecuteSql(stringSql.ToString(), Par);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0)
            {
                return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        public virtual DbModels.certcenterlog CreateModel(DataRow dr)
        {
            return new DbModels.certcenterlog
            {
                //
                id = Convert.ToInt32(dr["id"]),
                //
                url = dr["url"].ToString(),
                //
                reqdata = dr["reqdata"].ToString(),
                //
                userid = dr["userid"].ToString(),
                username = dr["username"].ToString(),
                //
                reqtime = Convert.ToDateTime(dr["reqtime"]),
                //
                ip = dr["ip"].ToString(),
                //
                opecontent = dr["opecontent"].ToString()
            };
        }

        public List<DbModels.certcenterlog> GetPage(XXF.Db.DbConn PubConn, int pno, int pagesize, string keywords, out int totalcount)
        {
            string basesql = "select {0} from certcenterlog ";
            string querysql = string.Format(basesql, " ROW_NUMBER() over (order by reqtime desc ) as rownum,id,url,reqdata,userid,username,reqtime,ip,opecontent");
            XXF.Db.SimpleProcedureParameter para = new SimpleProcedureParameter();
            string wherecon = "";
            if (!string.IsNullOrEmpty(keywords))
            {
                wherecon = " where userid like '%' +@keywords+'%' or username  like '%' +@keywords+'%' or url  like '%' +@keywords+'%'  or ip  like '%' +@keywords+'%' ";
                para.Add("@keywords", keywords);
            }
            DataSet ds = new DataSet();
            querysql = string.Concat("select A.* from (", querysql, wherecon, ") A", " where A.rownum between ", (pno - 1) * pagesize + 1, " and ", pagesize * pno);

            PubConn.SqlToDataSet(ds, querysql, para.ToParameters());
            List<DbModels.certcenterlog> listlog = new List<DbModels.certcenterlog>();
            foreach (DataRow a in ds.Tables[0].Rows)
            {
                listlog.Add(CreateModel(a));
            }
            totalcount = (int)PubConn.ExecuteScalar(string.Format(basesql, " count(*) ") + wherecon, para.ToParameters());
            return listlog;
        }

    }


}