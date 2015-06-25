using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace XXF.Db
{
    /// <summary>数据库连接类</summary>
    public abstract class DbConn : IDisposable 
    {
        /// <summary>数据库连接内部字段</summary>
        protected DbConnection _conn;

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
        public static DbConn CreateConn(string connectionString)
        {
            return CreateConn(DbType.SQLSERVER, connectionString);
        }

        /// <summary>创建连接</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <returns></returns>
        public static DbConn CreateConn(DbType dbtype)
        {
            return CreateConn(dbtype, "");
        }

        /// <summary>创建连接</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public static DbConn CreateConn(DbType dbtype, string connectionString)
        {
            DbConn cn = null;
            switch (dbtype)
            {
                case DbType.SQLSERVER:
                    cn = new DbConnSql();
                    break;
                case DbType.ORACLE:
                    cn = new DbConnOracle();
                    break;
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
        public static DbConn CreateConn(DbType dbtype, string AServerName, string ADatabaseName, string ALoginName, string ALoginPass)
        {
            return CreateConn(dbtype, CreateConnString(dbtype, AServerName, ADatabaseName, ALoginName, ALoginPass));
        }

        /// <summary>创建连接</summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="ACn">已有的连接</param>
        /// <returns></returns>
        public static DbConn CreateConn(DbType dbtype, DbConnection ACn)
        {
            DbConn cn = null;
            switch (dbtype)
            {
                case DbType.SQLSERVER:
                    cn = new DbConnSql();
                    break;
                case DbType.ORACLE:
                    cn = new DbConnOracle();
                    break;
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
        public static string CreateConnString(DbType dbtype,string AServerName, string ADatabaseName, string ALoginName, string ALoginPass)
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

        /// <summary>将DataSet中列的类型转换为DataField类型</summary>
        /// <param name="AType">原类型，一般取自DataSet.Table.Columns</param>
        /// <returns></returns>
        public static Db.FieldType TypeToFieldType(Type AType)
        {
            Db.FieldType ft;
            switch (AType.FullName)
            {
                case "System.Int16":
                    ft = Db.FieldType.Int16;
                    break;
                case "System.Int32":
                    ft = Db.FieldType.Int32;
                    break;
                case "System.Int64":
                    ft = Db.FieldType.Int64;
                    break;
                case "System.Single":
                    ft = Db.FieldType.Single;
                    break;
                case "System.Double":
                    ft = Db.FieldType.Double;
                    break;
                case "System.Decimal":
                    ft = Db.FieldType.Decimal;
                    break;
                case "System.String":
                    ft = Db.FieldType.String;
                    break;
                case "System.Byte[]":
                    ft = Db.FieldType.Image;
                    break;
                case "System.DateTime":
                    ft = Db.FieldType.DateTime;
                    break;
                case "System.Boolean":
                    ft = Db.FieldType.Boolean;
                    break;
                default:
                    throw new Exception("未知的类型[" + AType.FullName + "]!");
            }
            return ft;
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

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        public int ExecuteSql(string Sql, List<ProcedureParameter> ProcedurePar)
        {
            return ExecuteSql(Sql, CommandType.Text, ProcedurePar);
        }

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        public object ExecuteScalar(string Sql, List<ProcedureParameter> ProcedurePar)
        {
            return ExecuteScalar(Sql, CommandType.Text, ProcedurePar);
        }

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        public virtual void SqlBulkCopy(DataTable dt, string datatablename, string Sql, List<ProcedureParameter> ProcedurePar, int BatchSize = 0)
        {
        }

        public virtual void SqlBulkCopy(DataTable dt, string datatablename, string Sql,
                                         List<ProcedureParameter> ProcedurePar, Dictionary<string, string> mapps,
                                         int BatchSize = 0)
        {
        }

        /// <summary>
        /// 扩展支持返回datatable 车毅
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="ProcedurePar"></param>
        /// <returns></returns>
        public virtual DataTable SqlToDataTable(string Sql, List<ProcedureParameter> ProcedurePar)
        {
            DataSet ds = new DataSet();
            SqlToDataSet(ds, Sql, ProcedurePar);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        #region 虚方法

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

        /// <summary>用SQL语句返回记录集</summary>
        /// <param name="ds">记录集</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns></returns>
        public abstract void SqlToDataSet(DataSet ds, string Sql, List<ProcedureParameter> ProcedurePar);

        /// <summary>用SQL语句返回记录集</summary>
        /// <param name="ds">记录集</param>
        /// <param name="Sql">SQL语句</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public abstract void SqlToDataSet(DataSet ds, string Sql, List<ProcedureParameter> ProcedurePar, string TableName);

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        /// <param name="CmdType">命令类型</param>
        /// <returns></returns>
        public abstract int ExecuteSql(string Sql, CommandType CmdType, List<ProcedureParameter> ProcedurePar);

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        /// <param name="CmdType">命令类型</param>
        /// <returns></returns>
        public abstract object ExecuteScalar(string Sql, CommandType CmdType, List<ProcedureParameter> ProcedurePar);

        /// <summary>执行SQL语句</summary>
        /// <param name="Sql">查询语句</param>
        /// <param name="ImagePar">参数</param>
        /// <returns></returns>
        //public abstract int ExecuteSql(string Sql, List<ImageParameter> ImagePar);

        /// <summary>执行SQL语句</summary>
        /// <param name="ProcedureName">存储过程名称</param>
        /// <param name="ProcedurePar">参数</param>
        /// <returns></returns>
        public abstract int ExecuteProcedure(string ProcedureName, List<ProcedureParameter> ProcedurePar);

        /// <summary>用SQL语句返回DataReader</summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public abstract DbDataReader SqlToDbReader(string Sql, List<ProcedureParameter> ProcedurePar);

        /// <summary>取得刚刚插入数据库的种子序号</summary>
        /// <returns></returns>
        public abstract int GetIdentity();

        /// <summary>取得系统时间</summary>
        /// <returns></returns>
        public abstract DateTime GetServerDate();

        /// <summary>数据库中表是否存在</summary>
        /// <param name="aTableName">表名</param>
        /// <returns></returns>
        public abstract bool TableIsExist(string aTableName);

        /// <summary>数据库中表是否存在</summary>
        /// <param name="aDbName">数据库名</param>
        /// <param name="aTableName">表名</param>
        /// <returns></returns>
        public abstract bool TableIsExist(string aDbName, string aTableName);

        /// <summary>表中字段是否存在</summary>
        /// <param name="aTableName">表名</param>
        /// <param name="aFieldName">字段名</param>
        /// <returns></returns>
        public abstract bool FieldIsExist(string aTableName, string aFieldName);

        /// <summary>表中字段是否存在</summary>
        /// <param name="aDbName">数据库名</param>
        /// <param name="aTableName">表名</param>
        /// <param name="aFieldName">字段名</param>
        /// <returns></returns>
        public abstract bool FieldIsExist(string aDbName, string aTableName, string aFieldName);

        /// <summary>从多选字符串MultiChoiceStr转换成Sql条件</summary>
        /// <param name="AMultiChoiceStr">多选字符串</param>
        /// <param name="aField">字段</param>
        /// <param name="aStyle">类型 0-两边都不加  1-%左边加  2-右边加%  3-%两边加%</param>
        /// <param name="aPar">返回参数</param>
        /// <returns></returns>
        public abstract string GetMcsToSql(string AMultiChoiceStr, string aField, int aStyle, ref List<Db.ProcedureParameter> aPar);
        #endregion 虚方法
    }
}
