using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using XXF.Db;

namespace CertCenter.Models
{
    public class ApiDal
    {
        public readonly static ApiDal Instance = new ApiDal();

        public List<DbModels.api> GetCategoryApis(XXF.Db.DbConn PubConn, int apptype, int categoryid)
        {
            string sql = "SELECT  apiid,apptype,appgradeno,categoryid,apiname,apititle,area,controller,action,para,apidesc,freeze  FROM api where apptype=" + apptype + " and categoryid=" + categoryid;
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, null);
            List<DbModels.api> list = new List<DbModels.api>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(CreateModel(dr));
            }
            return list;
        }

        public List<DbModels.api> GetGradeApis(XXF.Db.DbConn PubConn, int apptype, int appgradeno)
        {
            string sql = "SELECT  apiid,apptype,appgradeno,categoryid,apiname,apititle,area,controller,action,para,apidesc,freeze  FROM api where apptype=" + apptype + " and appgradeno=" + appgradeno;
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, null);
            List<DbModels.api> list = new List<DbModels.api>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(CreateModel(dr));
            }
            return list;
        }

        public virtual bool Add(DbConn PubConn, Models.DbModels.api model)
        {
            model.apiname = model.apiname ?? "";
            model.apititle = model.apititle ?? "";
            model.area = model.area ?? "";
            model.controller = model.controller ?? "";
            model.action = model.action ?? "";
            model.para = model.para ?? "";
            model.apidesc = model.apidesc ?? "";
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					//应用类型
					new ProcedureParameter("@apptype",    model.apptype),
					//应用级别
					new ProcedureParameter("@appgradeno",    model.appgradeno),
					//
					new ProcedureParameter("@categoryid",    model.categoryid),
					//接口名
					new ProcedureParameter("@apiname",    model.apiname??""),
					//接口标题
					new ProcedureParameter("@apititle",    model.apititle??""),
					//area
					new ProcedureParameter("@area",    model.area??""),
					//controller
					new ProcedureParameter("@controller",    model.controller??""),
					//action
					new ProcedureParameter("@action",    model.action??""),
					//para
					new ProcedureParameter("@para",    model.para??""),
					//api说明
					new ProcedureParameter("@apidesc",    model.apidesc??"")  , 
					new ProcedureParameter("@freeze",    model.freeze)   
                };
            int rev = PubConn.ExecuteSql(@"insert into api(apptype,appgradeno,categoryid,apiname,apititle,area,controller,action,para,apidesc,freeze)
										   values(@apptype,@appgradeno,@categoryid,@apiname,@apititle,@area,@controller,@action,@para,@apidesc,@freeze)", Par);
            return rev == 1;

        }

        public virtual bool Edit(DbConn PubConn, Models.DbModels.api model)
        {
            model.apiname = model.apiname ?? "";
            model.apititle = model.apititle ?? "";
            model.area = model.area ?? "";
            model.controller = model.controller??"";
            model.action = model.action ??"";
            model.para = model.para ?? "";
            model.apidesc = model.apidesc ?? "";
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
                    
					//权限id
					new ProcedureParameter("@apiid",    model.apiid),
					//应用类型
					new ProcedureParameter("@apptype",    model.apptype),
					//应用级别
					new ProcedureParameter("@appgradeno",    model.appgradeno),
					//
					new ProcedureParameter("@categoryid",    model.categoryid),
					//接口名
					new ProcedureParameter("@apiname",    model.apiname??""),
					//接口标题
					new ProcedureParameter("@apititle",    model.apititle??""),
					//area
					new ProcedureParameter("@area",    model.area??""),
					//controller
					new ProcedureParameter("@controller",    model.controller??""),
					//action
					new ProcedureParameter("@action",    model.action??""),
					//para
					new ProcedureParameter("@para",    model.para??""),
					//api说明
					new ProcedureParameter("@apidesc",    model.apidesc??""),
                    
					new ProcedureParameter("@freeze",    model.freeze)
            };
            Par.Add(new ProcedureParameter("@id", model.apiid));

            int rev = PubConn.ExecuteSql("update api set apptype=@apptype,appgradeno=@appgradeno,categoryid=@categoryid,apiname=@apiname,apititle=@apititle,area=@area,controller=@controller,action=@action,para=@para,apidesc=@apidesc,freeze=@freeze where apiid=@apiid", Par);
            return rev == 1;

        }

        public virtual bool Delete(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@apiid", id));

            string Sql = "delete from api where apiid=@apiid";
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

        public virtual Models.DbModels.api Get(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", ProcParType.Int32, 4, id));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from api s where s.apiid=@id");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0)
            {
                return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        public virtual Models.DbModels.api CreateModel(DataRow dr)
        {
            return new Models.DbModels.api
            {
                //权限id
                apiid = Convert.ToInt32(dr["apiid"]),
                //应用类型
                apptype = Convert.ToInt32(dr["apptype"]),
                //应用级别
                appgradeno = Convert.ToInt32(dr["appgradeno"]),
                //
                categoryid = Convert.ToInt32(dr["categoryid"]),
                //接口名
                apiname = dr["apiname"].ToString(),
                //接口标题
                apititle = dr["apititle"].ToString(),
                //area
                area = dr["area"].ToString(),
                //controller
                controller = dr["controller"].ToString(),
                //action
                action = dr["action"].ToString(),
                //para
                para = dr["para"].ToString(),
                //api说明
                apidesc = dr["apidesc"].ToString(),
                freeze = Convert.ToInt32(dr["freeze"])
            };
        }

        public Models.DbModels.api Get(DbConn PubConn, string area, string controller, string action,int apptype)
        {
            XXF.Db.SimpleProcedureParameter para = new SimpleProcedureParameter();
            para.Add("area", area ?? "");
            para.Add("controller", controller ?? "");
            para.Add("action", action ?? "");

            para.Add("@apptype", apptype);
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from api s where s.area=@area and s.controller=@controller and s.action=@action and s.apptype=@apptype");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), para.ToParameters());
            if (ds != null && ds.Tables.Count > 0)
            {
                return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        public Models.DbModels.api Get(DbConn PubConn, string apiname, int apptype)
        {
            XXF.Db.SimpleProcedureParameter para = new SimpleProcedureParameter();
            para.Add("area", apiname ?? "");
           
            para.Add("@apptype", apptype);
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from api s where s.appname=@appname and s.apptype=@apptype");
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), para.ToParameters());
            if (ds != null && ds.Tables.Count > 0)
            {
                return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }
    }
}