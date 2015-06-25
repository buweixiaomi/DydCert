using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace XXF.Db
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public static class DbConfig
    {

        /// <summary>取得配置信息</summary>
        /// <param name="Name">配置名称</param>
        /// <returns></returns>
        public static string GetConfig(string Name)
        {
            return LibConvert.NullToStr(ConfigurationManager.AppSettings[Name]);
        }

        /// <summary>数据库类型</summary>
        public static Db.DbType DbType
        {
            get
            {
                return (Db.DbType)LibConvert.StrToInt(GetConfig("DbType"));
            }
        }

        /// <summary>连接字符串</summary>
        public static string ConnectionString
        {
            get
            {
                /*兼容ConnectionString单条配置或者原来的多项配置方式*/
                if (!string.IsNullOrEmpty(GetConfig("DbServer")))
                {
                    string Temple = "Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};Min Pool Size=10;Max Pool Size=200;";
                    return string.Format(Temple, GetConfig("DbServer"), GetConfig("DbName"), GetConfig("DbUser"), GetConfig("DbPass"));
                }
                else {
                    return GetConfig("ConnectionString");
                }
            }
        }

        /// <summary>创建数据库连接</summary>
        /// <returns></returns>
        public static Db.DbConn CreateConn()
        {
            return Db.DbConn.CreateConn(DbType, ConnectionString);
        }

        /// <summary>创建数据库连接</summary>
        /// <returns></returns>
        public static Db.DbConn CreateConn(DbType dbtype,string connectionString)
        {
            return Db.DbConn.CreateConn(dbtype, connectionString);
        }

        /// <summary>创建数据库连接</summary>
        /// <returns></returns>
        public static Db.DbConn CreateConn(string connectionString)
        {
            return Db.DbConn.CreateConn(DbType.SQLSERVER, connectionString);
        }

        //public static ApiWebContext BuildWebContext(Controller control, Lib.Db.DbConn PubConn)
        //{
        //    string userIP = "";
        //    if (control.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
        //        userIP = control.Request.ServerVariables["REMOTE_ADDR"];
        //    else
        //        userIP = control.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //    if (userIP == null || userIP == "")
        //        userIP = control.Request.UserHostAddress;
        //    ApiWebContext ac = new ApiWebContext();
        //        ac.Zybm = GetZybm(control);
        //        ac.Address = userIP;
        //        ac.Host = control.Request.ServerVariables["HTTP_HOST"];
        //        ac.Url = control.Request.ServerVariables["URL"];
        //        ac.SessionId = control.Session.SessionID;
        //    if (GetConfig("ServiceLog") == "1")
        //    {
        //        Pub.AddSystemLog(PubConn, ac, "buildwebcontent", "createac","");
        //    }
        //    return ac;
        //}
    }
}
