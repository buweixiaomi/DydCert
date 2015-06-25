using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CertCenter.Models
{
    public class AppDal
    {
        private readonly string basesql = " select {0} from app,appgrade where app.appgradeno=appgrade.appgradeno and app.apptype=appgrade.apptype  ";
        public static readonly AppDal Instance = new AppDal();

        public List<Models.DbModels.app> GetList(XXF.Db.DbConn PubConn, int pno, int pagesize, string keywords, out int totalcount)
        {
            totalcount = 0;
            List<Models.DbModels.app> listapp = new List<DbModels.app>();
            string sql = string.Format(basesql, " ROW_NUMBER() over (order by appid) as rownum, appid,appname,appsecret,app.apptype,appgrade.appgradeno,appgrade.appgradename,appdesc,freeze "); ;
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            string con = "";
            if (!string.IsNullOrEmpty(keywords))
            {
                con = " and app.appid like '%' + @keywords + '%' or app.appname like '%' + @keywords + '%' or appname like '%' + @keywords + '%' ";
                para.Add("@keywords", keywords);
                sql = sql + con;
            }
            string querysql = string.Concat("select A.* from (", sql, ") A ", " where A.rownum between ", (pno - 1) * pagesize + 1, " and ", pno * pagesize);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, querysql, para.ToParameters());
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                listapp.Add(FillModel(dr));
            }
            totalcount = (int)PubConn.ExecuteScalar(string.Format(basesql, " count(*) ") + con, para.ToParameters());

            return listapp;
        }

        /// <summary>
        /// 同时会填充等级名称
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public Models.DbModels.app FillModel(DataRow dr)
        {
            Models.DbModels.app model = new DbModels.app();
            model.appid = dr["appid"].ToString();
            model.appsecret = dr["appsecret"].ToString();
            model.apptype = Convert.ToInt32(dr["apptype"]);
            model.appgradeno = Convert.ToInt32(dr["appgradeno"]);
            model.appdesc = dr["appdesc"].ToString();
            model.appgrade = new DbModels.appgrade();
            model.appname = dr["appname"].ToString();
            model.appgrade.appgradeno = model.appgradeno;
            model.appgrade.appgradename = dr["appgradename"].ToString();
            return model;
        }

        //get
        public Models.DbModels.app GetAppInfo(XXF.Db.DbConn PubConn, string appid)
        {
            string sql = string.Format(basesql, "  appid,appname,appsecret,app.apptype,appgrade.appgradeno,appgrade.appgradename,appdesc,freeze ");
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@appid", appid);
            sql += " and appid=@appid";
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            return FillModel(ds.Tables[0].Rows[0]);
        }

        public int AddApp(XXF.Db.DbConn PubConn, Models.DbModels.app model)
        {
            if (string.IsNullOrEmpty(model.appid))
            {
                model.appid = XXF.Db.LibString.MakeRandomNumber(16).ToLower();
            }
            if (ExitAppid(PubConn, model.appid))
            {
                return -2;
            }
            if (string.IsNullOrEmpty(model.appsecret))
            {
                model.appsecret = Guid.NewGuid().ToString().Replace("-", "");
            }

            string sql = "insert into app(appid,appname,apptype,appgradeno,appsecret,appdesc,freeze) values(@appid,@appname,@apptype,@appgradeno,@appsecret,@appdesc,@freeze)";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@appid", model.appid);
            para.Add("@appsecret", model.appsecret);
            para.Add("@appname", model.appname);
            para.Add("@apptype", model.apptype);
            para.Add("@appgradeno", model.appgradeno);
            para.Add("@freeze", model.freeze);
            para.Add("@appdesc", model.appdesc ?? "");

            int r = PubConn.ExecuteSql(sql, para.ToParameters());
            return r;
        }

        public bool ExitAppid(XXF.Db.DbConn PubConn, string appid)
        {
            string sql = "select count(*) from app where appid=@appid";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@appid", appid);
            int r = (int)PubConn.ExecuteScalar(sql, para.ToParameters());
            return r == 1;
        }


        public int UpdateApp(XXF.Db.DbConn PubConn, Models.DbModels.app model)
        {
            string sql = "update app   set appname=@appname,appsecret=@appsecret,appdesc=@appdesc ,freeze=@freeze ,apptype = @apptype,appgradeno =@appgradeno where appid=@appid";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@appid", model.appid);
            para.Add("@appsecret", model.appsecret);
            para.Add("@appname", model.appname);
            para.Add("@apptype", model.apptype);
            para.Add("@appgradeno", model.appgradeno);
            para.Add("@freeze", model.freeze);
            para.Add("@appdesc", model.appdesc ?? "");

            int r = PubConn.ExecuteSql(sql, para.ToParameters());
            return r;
        }

        public int DeletApp(XXF.Db.DbConn PubConn, string appid)
        {
            string sql = "delete from app where appid=@appid";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@appid", appid); 
            int r = PubConn.ExecuteSql(sql, para.ToParameters());
            return r;
        }
    }
}