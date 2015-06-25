using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;
using System.Reflection;
using DapperExtensions.Sql;

namespace XXF.Db.Dapper
{
    public abstract class DbConnFactory : IDisposable
    {
        /// <summary>数据库连接内部字段</summary>
        protected DbConnection _conn;
        
        /// <summary>
        /// map扩展数据库对象
        /// </summary>
        public IDatabase _iDb;

        protected bool _isWatchTime = true;

        /// <summary>
        /// 是否监控sql耗时
        /// </summary>
        public bool IsWatchTime { get { return _isWatchTime; } set { _isWatchTime = value; } }

        #region 属性

        /// <summary>数据库类型</summary>
        protected DbType _dbtype;

        /// <summary>数据库类型
        /// </summary>
        public DbType DbType
        {
            get { return _dbtype; }
            set { _dbtype = value; }
        }

        /// <summary>连接字符串
        /// </summary>
        public string ConnString
        {
            get { return _conn.ConnectionString; }
            set { _conn.ConnectionString = value; }
        }
        #endregion 属性

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="connectionString">数据库类型:默认sqlserver</param>
        /// <returns></returns>
        public static DbConnFactory CreateConn(string connectionString)
        {
            return CreateConn(DbType.SQLSERVER, connectionString);
        }

        /// <summary>创建连接</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <returns></returns>
        public static DbConnFactory CreateConn(DbType dbtype)
        {
            return CreateConn(dbtype, "");
        }

        /// <summary>创建连接</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static DbConnFactory CreateConn(DbType dbtype, string connectionString)
        {
            DbConnFactory cn = null;
            switch (dbtype)
            {
                case DbType.SQLSERVER:
                    cn = new DbConnSql();
                    break;
                //case DbType.ORACLE:
                //    cn = new DbConnOracle();
                //    break;
                default:
                    throw new Exception("该数据库类型不适合使用CreateConn，请用new创建！");
            }
            cn.ConnString = connectionString;

            return cn;
        }

        /// <summary>创建连接</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="AServerName">服务器名称</param>
        /// <param name="ADatabaseName">数据库名称</param>
        /// <param name="ALoginName">用户</param>
        /// <param name="ALoginPass">密码</param>
        /// <returns></returns>
        public static DbConnFactory CreateConn(DbType dbtype, string AServerName, string ADatabaseName, string ALoginName, string ALoginPass)
        {
            return CreateConn(dbtype, CreateConnString(dbtype, AServerName, ADatabaseName, ALoginName, ALoginPass));
        }

        /// <summary>创建连接</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="ACn">已有的连接</param>
        /// <returns></returns>
        public static DbConnFactory CreateConn(DbType dbtype, DbConnection ACn)
        {
            DbConnFactory cn = null;
            switch (dbtype)
            {
                case DbType.SQLSERVER:
                    cn = new DbConnSql();
                    break;
                //case DbType.ORACLE:
                //    cn = new DbConnOracle();
                //    break;
                default:
                    throw new Exception("该数据库类型不适合使用CreateConn，请用new创建！");
            }
            cn._conn = ACn;
            return cn;
        }

        /// <summary>取得数据库连接字符串(SQL传所有参数、ORACLE传AServerName ALoginName ALoginPass、ACCESS传ADatabaseName ALoginPass)</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="AServerName">服务器名</param>
        /// <param name="ADatabaseName">数据库名</param>
        /// <param name="ALoginName">用户</param>
        /// <param name="ALoginPass">密码</param>
        /// <returns></returns>
        public static string CreateConnString(DbType dbtype, string AServerName, string ADatabaseName, string ALoginName, string ALoginPass)
        {
            switch (dbtype)
            {
                case DbType.SQLSERVER:
                    if (ADatabaseName == "") ADatabaseName = "master";
                    return "Data Source=" + AServerName + ";Initial Catalog=" + ADatabaseName + ";Persist Security Info=True;User ID=" + ALoginName + ";Password=" + ALoginPass;
                case DbType.ORACLE:
                    return "Data Source=" + AServerName + ";Persist Security Info=True;User ID=" + ALoginName + ";Unicode=True;Password=" + ALoginPass;
            }
            return "";
        }

        /// <summary>
        /// 打开映射扩展对象
        /// </summary>
        public void OpenMapExt()
        {
            var config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect());
            var sqlGenerator = new SqlGeneratorImpl(config);
            _iDb = new Database(_conn, sqlGenerator);;
        }

        /// <summary>打开数据库连接
        /// </summary>
        public void Open()
        {
            //string Err = "";
            //if (!Lib.Sys.GetRockState(0,ref Err))
            //    throw new Exception(Err);
            _conn.Open();
        }

        /// <summary>关闭数据库连接
        /// </summary>
        public void Close()
        {
            _conn.Close();
        }

        /// <summary>释放
        /// </summary>
        public void Dispose()
        {
            _conn.Close();
            _conn.Dispose();
            _conn = null;
        }

        /// <summary>取得数据库连接对象</summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            return _conn;
        }

        /// <summary>启动事务</summary>
        /// <returns></returns>
        public abstract void BeginTransaction();

        /// <summary>提交事务
        /// </summary>
        public abstract void Commit();

        /// <summary>回滚事务
        /// </summary>
        public abstract void Rollback();

        /// <summary>取得事务对象</summary>
        /// <returns></returns>
        public abstract DbTransaction GetTransaction();

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        public abstract DateTime GetServerDate();
    }
}
