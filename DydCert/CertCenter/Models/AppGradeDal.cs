using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CertCenter.Models
{
    public class AppGradeDal
    {
        public readonly static AppGradeDal Instance = new AppGradeDal();

        public List<DbModels.appgrade> GetGrades(XXF.Db.DbConn PubConn, int typ)
        {
            List<DbModels.appgrade> listmodel = new List<DbModels.appgrade>();
            string sql = "select apptype,appgradeno,appgradename,appgradedesc from appgrade where apptype=@apptype order by appgradeno asc";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", typ);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DbModels.appgrade item = new DbModels.appgrade();
                item.apptype = Convert.ToInt32(dr["apptype"]);
                item.appgradeno = Convert.ToInt32(dr["appgradeno"]);
                item.appgradename = dr["appgradename"].ToString();
                item.appgradedesc = dr["appgradedesc"].ToString();
                listmodel.Add(item);
            }
            return listmodel;
        }

        public DbModels.appgrade GetGradeInfo(XXF.Db.DbConn PubConn, int apptype, int appgradeno)
        {
            string sql = "select apptype,appgradeno,appgradename,appgradedesc from appgrade where apptype=@apptype and appgradeno=@appgradeno ";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", apptype);
            para.Add("@appgradeno", appgradeno);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            DataRow dr = ds.Tables[0].Rows[0];
            DbModels.appgrade item = new DbModels.appgrade();
            item.apptype = Convert.ToInt32(dr["apptype"]);
            item.appgradeno = Convert.ToInt32(dr["appgradeno"]);
            item.appgradename = dr["appgradename"].ToString();
            item.appgradedesc = dr["appgradedesc"].ToString();
            return item;
        }

        public int AddGrade(XXF.Db.DbConn PubConn, Models.DbModels.appgrade model)
        {
            model.appgradedesc = model.appgradedesc ?? "";
            string getcatid = "select isnull(max(appgradeno),0)+1 from appgrade where apptype=" + model.apptype;
            model.appgradeno = Convert.ToInt32(PubConn.ExecuteScalar(getcatid, null));
            string addsql = "insert into appgrade(apptype,appgradeno,appgradename,appgradedesc) values(@apptype,@appgradeno,@appgradename,@appgradedesc)";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", model.apptype);
            para.Add("@appgradeno", model.appgradeno);
            para.Add("@appgradename", model.appgradename);
            para.Add("@appgradedesc", model.appgradedesc);
            int r = PubConn.ExecuteSql(addsql, para.ToParameters());
            return r;
        }

        public int UpdateGrade(XXF.Db.DbConn PubConn, Models.DbModels.appgrade model)
        {
            model.appgradedesc = model.appgradedesc ?? "";
            string sql = "update  appgrade set appgradename=@appgradename,appgradedesc=@appgradedesc where  apptype=@apptype and appgradeno=@appgradeno";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", model.apptype);
            para.Add("@appgradeno", model.appgradeno);
            para.Add("@appgradename", model.appgradename);
            para.Add("@appgradedesc", model.appgradedesc);
            int r = PubConn.ExecuteSql(sql, para.ToParameters());
            return r;
        }

        public int DeleteGrade(XXF.Db.DbConn PubConn, int apptype, int appgradeno)
        {
            string countsql = "  select COUNT(*) from api where apptype=" + apptype + " and appgradeno=" + appgradeno + "";
            int c = (int)PubConn.ExecuteScalar(countsql, null);
            if (c != 0)
                return -2;
            string sql = "delete from appgrade where  apptype=@apptype and appgradeno=@appgradeno";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", apptype);
            para.Add("@appgradeno", appgradeno);
            int r = PubConn.ExecuteSql(sql, para.ToParameters());
            return r;
        }

    }
}