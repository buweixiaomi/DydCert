using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XXF.Db;

namespace CertCenter.Models
{
    public partial class TokenDal
    {
        public static readonly TokenDal Instance = new TokenDal();

        /// <summary>
        /// createtime expiresetime不用传。
        /// </summary>
        /// <param name="PubConn"></param>
        /// <param name="model"></param>
        /// <param name="tokentype"></param>
        /// <returns></returns>
        public virtual bool Add(DbConn PubConn, DbModels.tb_token model, Models.DbModels.TokenType tokentype)
        {
            DateTime nowtime = PubConn.GetServerDate();
            model.createtime = nowtime;
            model.expires = nowtime.AddMinutes(GetExpiresminutes(tokentype));
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					
					//
					new ProcedureParameter("@token",    model.token),
					//
					new ProcedureParameter("@userid",    model.userid),
					new ProcedureParameter("@id",    model.id),
					//
					new ProcedureParameter("@username",    model.username),
					//
					new ProcedureParameter("@appid",    model.appid),
					//
					new ProcedureParameter("@createtime",   model.createtime),
					//
					new ProcedureParameter("@expires", model.expires  )   
                };
            int rev = PubConn.ExecuteSql("insert into " + tokentype.ToString() + " (token,userid,id,username,appid,createtime,expires)" +
                                         "  values(@token,@userid,@id,@username,@appid,@createtime,@expires)", Par);
            return rev == 1;

        }

        /// <summary>  得到过期时间 </summary>
        /// <param name="tokentype"></param>
        /// <returns></returns>
        public int GetExpiresminutes(Models.DbModels.TokenType tokentype)
        {
            int minute = 60;
            string stringminutes ="";
            switch (tokentype)
            {
                case CertCenter.Models.DbModels.TokenType.usertoken:
                    stringminutes = XXF.Db.DbConfig.GetConfig("UserTokenExpires");
                    if (string.IsNullOrEmpty(stringminutes))
                    {
                        return 2000;
                    }
                    minute = Convert.ToInt32(stringminutes);
                    break;
                case CertCenter.Models.DbModels.TokenType.shoptoken:
                     stringminutes = XXF.Db.DbConfig.GetConfig("ShopTokenExpires");
                    if (string.IsNullOrEmpty(stringminutes))
                    {
                        return 1000;
                    }
                    minute = Convert.ToInt32(stringminutes);
                    break;
                case CertCenter.Models.DbModels.TokenType.managetoken:
                     stringminutes = XXF.Db.DbConfig.GetConfig("ManageTokenExpires");
                    if (string.IsNullOrEmpty(stringminutes))
                    {
                        return 60;
                    }
                    minute = Convert.ToInt32(stringminutes);
                    break;
                default:
                    break;
            }
            return minute;
        }



        public Models.DbModels.tb_token GetToken(DbConn PubConn, string token, DbModels.TokenType tokentype)
        {
            string sql = " select token,userid,id,username,appid,createtime,expires from " + tokentype.ToString() + " where token=@token";
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@token", token));
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, Par);

            if (ds.Tables[0].Rows.Count != 1)
                return null;
            return CreateModel(ds.Tables[0].Rows[0]);
        }

        public CertCenter.Models.DbModels.tb_token GetToken(XXF.Db.DbConn PubConn, string userid, string appid, CertCenter.Models.DbModels.TokenType tokentype)
        {
            string sql = " select token,userid,id,username,appid,createtime,expires from " + tokentype.ToString() + " where userid=@userid and appid=@appid";
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@userid", userid));
            Par.Add(new ProcedureParameter("@appid", appid));
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, sql, Par);

            if (ds.Tables[0].Rows.Count != 1)
                return null;
            return CreateModel(ds.Tables[0].Rows[0]);
        }


        public virtual bool Edit(DbConn PubConn, DbModels.tb_token model, DbModels.TokenType tokentype)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
					//
					new ProcedureParameter("@token",    model.token),
					//
					new ProcedureParameter("@userid",    model.userid),
                    //
					new ProcedureParameter("@id",    model.id),
					//
					new ProcedureParameter("@username",    model.username),
					//
					new ProcedureParameter("@appid",    model.appid),
					//
					new ProcedureParameter("@createtime",    model.createtime),
					//
					new ProcedureParameter("@expires",    model.expires)
            };
            int rev = PubConn.ExecuteSql("update " + tokentype.ToString() + " set userid=@userid,id=@id,username=@username,appid=@appid,createtime=@createtime,expires=@expires where token=@token", Par);
            return rev == 1;

        }

        public virtual bool Delete(DbConn PubConn, string id, DbModels.TokenType tokentype)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@token", id));

            string Sql = "delete from " + tokentype.ToString() + " where token=@token";
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

        /// <summary>清除过期token</summary>
        public static void DeleteExpiresToken()
        {
            try
            {
                string sql = @"delete from usertoken where expires<GETDATE() 
                                delete from managetoken where expires<GETDATE()
                                delete from shoptoken where expires<GETDATE()";
                using (XXF.Db.DbConn PubConn = XXF.Db.DbConfig.CreateConn())
                {
                    PubConn.Open();//打开基本
                    PubConn.ExecuteSql(sql, null);
                }
            }
            catch (Exception ex) { }
        }

        public List<Models.DbModels.tb_token> GetByPage(XXF.Db.DbConn PubConn, int pno, int pagesize, string keywords, DbModels.TokenType tokentype, out int totalcount)
        {
            List<DbModels.tb_token> tokenlist = new List<DbModels.tb_token>();
            string basesql = "select {0} from " + tokentype.ToString() + " ";
            string whercon = "";
            XXF.Db.SimpleProcedureParameter para = new SimpleProcedureParameter();
            string querysql = string.Format(basesql, "ROW_NUMBER() over (order by createtime desc) as rownum, token,userid,id,username,appid,createtime,expires");
            if (!string.IsNullOrEmpty(keywords))
            {
                para.Add("@keywords", keywords);
                whercon = " where token=@keywords or userid like '%' + @keywords + '%' or username  like '%' + @keywords + '%' or appid  like '%' + @keywords + '%' ";
            }

            querysql = string.Concat("select A.* from (", querysql, whercon, ") A ", " where A.rownum between ", (pno - 1) * pagesize + 1, " and ", pagesize * pno);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, querysql, para.ToParameters());
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                tokenlist.Add(CreateModel(dr));
            }
            totalcount = (int)PubConn.ExecuteScalar(string.Format(basesql, " count(*) ") + whercon, para.ToParameters());
            return tokenlist;
        }


        public virtual DbModels.tb_token CreateModel(DataRow dr)
        {
            return new DbModels.tb_token
            {
                //
                token = dr["token"].ToString(),
                //
                userid = dr["userid"].ToString(),
                //
                id = dr["id"].ToString(),
                //
                username = dr["username"].ToString(),
                //
                appid = dr["appid"].ToString(),
                //
                createtime = Convert.ToDateTime(dr["createtime"]),
                //
                expires = Convert.ToDateTime(dr["expires"])
            };
        }
    }
}