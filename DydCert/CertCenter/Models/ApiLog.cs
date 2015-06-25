using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertCenter.Models.DbModels
{
    public class WorklogDal
    {
        public static void AddLog(XXF.Db.DbConn PubConn,DbModels.apilog log)
        {
            XXF.Db.SimpleProcedureParameter para = new XXF.Db.SimpleProcedureParameter();
            para.Add("@reqsource", log.reqsource);
            para.Add("@url", log.url);
            para.Add("@reqpara", log.reqpara);
            para.Add("@token", log.token);
            para.Add("@appid", log.appid);
        //    para.Add("@appname", log.appname);
            para.Add("@userid", log.userid);
            para.Add("@username", log.username);
            para.Add("@reqdate", log.reqdate);
            para.Add("@opecontent", log.opecontent);

            string sql = "INSERT INTO operationlog (reqsource  ,url  ,reqpara ,token ,appid ,appname,userid ,username ,reqdate ,opecontent)"+
                             "VALUES (@reqsource,@url, @reqpara,@token,@appid,@appname,@userid, @username,@reqdate,@opecontent)";
            PubConn.ExecuteSql(sql, para.ToParameters());
        }

        public static DbModels.apilog BuildBaseLog(Controller c)
        {
            return null;
            DbModels.apilog log = new apilog();
            log.reqdate = DateTime.Now;
        }

    }
}