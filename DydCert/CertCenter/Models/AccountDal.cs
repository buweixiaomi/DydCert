using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CertCenter.Models
{
    public class AccountDal
    {
        public static readonly AccountDal Instance = new AccountDal();


        /// <summary>根据userid得到管理员工信息</summary>
        public DbModels.manage getManage(XXF.Db.DbConn PubConn, string userid)
        {
            string sql = "select ygbh,ygmc,ygmm,ygzt,sfsc from tb_userinfo where ygbh=@userid";
            DataSet ds = new DataSet();
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@userid", userid);
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count != 1)
                return null;
            DbModels.manage model = new DbModels.manage();
            model.userid = ds.Tables[0].Rows[0]["ygbh"].ToString();
            model.id = model.userid;
            model.username = ds.Tables[0].Rows[0]["ygmc"].ToString();
            model.pwd = privateToDeDES(ds.Tables[0].Rows[0]["ygmm"].ToString());
            model.freeze = Convert.ToInt32(ds.Tables[0].Rows[0]["sfsc"]);
            if (model.freeze == 0)
            {
                model.freeze = 1;
            }
            else
            {
                model.freeze = 0;
            }
            return model;
        }

        /// <summary>根据userid得到管理员工信息</summary>
        public DbModels.manage getShop(XXF.Db.DbConn PubConn, string userid)
        {
            string sql = "SELECT  f_shzh,f_shsj,f_shmc,f_dlmm,f_sfdj FROM [tb_shop] where f_shzh=@userid or f_shsj=@userid";
            DataSet ds = new DataSet();
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@userid", userid);
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count != 1)
                return null;
            DbModels.manage model = new DbModels.manage();
            model.userid = ds.Tables[0].Rows[0]["f_shzh"].ToString();
            model.id = model.userid;
            model.username = ds.Tables[0].Rows[0]["f_shmc"].ToString();
            model.pwd = privateToDeDES(ds.Tables[0].Rows[0]["f_dlmm"].ToString());
            model.freeze = Convert.ToInt32(ds.Tables[0].Rows[0]["f_sfdj"]);
            return model;
        }
        /// <summary></summary>
        public DbModels.manage getUser(XXF.Db.DbConn PubConn, string userid)
        {
            string sql = "select f_id, f_yhzh,f_yhxm,f_dlmm,f_sfdj from tb_customer where f_yhzh=@userid";
            DataSet ds = new DataSet();
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@userid", userid);
            PubConn.SqlToDataSet(ds, sql, para.ToParameters());
            if (ds.Tables[0].Rows.Count != 1)
                return null;
            DbModels.manage model = new DbModels.manage();
            model.userid = ds.Tables[0].Rows[0]["f_yhzh"].ToString();
            model.id = ds.Tables[0].Rows[0]["f_id"].ToString();
            model.username = ds.Tables[0].Rows[0]["f_yhxm"].ToString();
            model.pwd = privateToDeDES(ds.Tables[0].Rows[0]["f_dlmm"].ToString());
            model.freeze = Convert.ToInt32(ds.Tables[0].Rows[0]["f_sfdj"]);
            return model;
        }

        private string privateToDeDES(string s)
        {
            try
            {
                string r = XXF.Db.LibCrypto.DeDES(s);
                return r;
            }
            catch (Exception ex)
            {
                return s;
                throw new Exception("密码不是密文密码");
            }
        }

        internal string getUserName(XXF.Db.DbConn PubConn, string userid, DbModels.TokenType tokentype)
        {
            CertCenter.Models.DbModels.manage model = null;
            switch (tokentype)
            {
                case DbModels.TokenType.usertoken: 
                    model = getUser(PubConn, userid);break;
                case DbModels.TokenType.managetoken:
                    model = getManage(PubConn, userid);
                    break;
                case DbModels.TokenType.shoptoken:
                    model = getShop(PubConn, userid);
                    break;
            }
            if (model == null)
                return "";
            return model.username;

        }
    }
}