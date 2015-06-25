using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using DapperExtensions;

namespace XXF.Db.Dapper
{
    class DbConnSql : DbConnFactory
    {
        private SqlConnection privateconn;
        private SqlTransaction ts = null;

        public DbConnSql()
        {
            privateconn = new SqlConnection();
            _conn = privateconn;
            _dbtype = DbType.SQLSERVER;
        }

        public override void BeginTransaction()
        {
            ts = privateconn.BeginTransaction();
        }

        public override void Commit()
        {
            try
            {
                ts.Commit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ts = null;
            }
        }

        public override void Rollback()
        {
            try
            {
                if (ts != null) ts.Rollback();
            }
            finally
            {
                ts = null;
            }
        }

        public override DbTransaction GetTransaction()
        {
            return ts;
        }

        public override DateTime GetServerDate()
        {
            var r = privateconn.Query<DateTime>("select GetDate() as aDate", transaction: ts).First();//参数要一一对应

            return r;
        }
    }
}
