using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CertCenter.Models
{
    public class AppcategoryDal
    {
        public static readonly AppcategoryDal Instance = new AppcategoryDal();

        public List<Models.DbModels.appcategory> GetAppTypeCategorys(XXF.Db.DbConn PubConn, int typeid)
        {
            List<DbModels.appcategory> listmodel = new List<DbModels.appcategory>();
            string sql = "select apptype,categoryid,categorytitle, categorydesc from appcategory where apptype=@apptype order by categoryid asc";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", typeid);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DbModels.appcategory item = new DbModels.appcategory();
                item.apptype = Convert.ToInt32(dr["apptype"]);
                item.categoryid = Convert.ToInt32(dr["categoryid"]);
                item.categorytitle = dr["categorytitle"].ToString();
                item.categorydesc = dr["categorydesc"].ToString();
                listmodel.Add(item);
            }
            return listmodel;
        }


        public Models.DbModels.appcategory GetCategoryInfo(XXF.Db.DbConn PubConn, int typeid, int categoryid)
        {
            string sql = "select apptype,categoryid,categorytitle, categorydesc from appcategory where apptype=@apptype  and categoryid=@categoryid";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", typeid);
            para.Add("@categoryid", categoryid);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            DataRow dr = ds.Tables[0].Rows[0];
            DbModels.appcategory item = new DbModels.appcategory();
            item.apptype = Convert.ToInt32(dr["apptype"]);
            item.categoryid = Convert.ToInt32(dr["categoryid"]);
            item.categorytitle = dr["categorytitle"].ToString();
            item.categorydesc = dr["categorydesc"].ToString();
            return item;
        }

        public int Addcategory(XXF.Db.DbConn PubConn, Models.DbModels.appcategory model)
        {
            model.categorydesc = model.categorydesc ?? "";
            string getcatid = "select isnull(max(categoryid),0)+1 from appcategory where apptype=" + model.apptype;
            model.categoryid = Convert.ToInt32(PubConn.ExecuteScalar(getcatid, null));
            string addsql = "insert into appcategory(apptype,categoryid,categorytitle,categorydesc) values(@apptype,@categoryid,@categorytitle,@categorydesc)";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", model.apptype);
            para.Add("@categoryid", model.categoryid);
            para.Add("@categorytitle", model.categorytitle);
            para.Add("@categorydesc", model.categorydesc);
            int r = PubConn.ExecuteSql(addsql, para.ToParameters());
            return r;
        }

        public int UpdateCategory(XXF.Db.DbConn PubConn, Models.DbModels.appcategory model)
        {
            model.categorydesc = model.categorydesc ?? "";
            string sql = "update  appcategory set categorytitle=@categorytitle,categorydesc=@categorydesc where  apptype=@apptype and categoryid=@categoryid";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", model.apptype);
            para.Add("@categoryid", model.categoryid);
            para.Add("@categorytitle", model.categorytitle);
            para.Add("@categorydesc", model.categorydesc);
            int r = PubConn.ExecuteSql(sql, para.ToParameters());
            return r;
        }

        public int DeleteCategory(XXF.Db.DbConn PubConn, int apptype, int categoryid)
        {
            string countsql = "  select COUNT(*) from gradepermission where apptype=" + apptype + " and categoryid=" + categoryid + "";
            int c = (int)PubConn.ExecuteScalar(countsql, null);
            if (c != 0)
                return -2;
            string sql = "delete from appcategory where  apptype=@apptype and categoryid=@categoryid";
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@apptype", apptype);
            para.Add("@categoryid", categoryid);
            int r = PubConn.ExecuteSql(sql, para.ToParameters());
            return r;
        }
    }
}