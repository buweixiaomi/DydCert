using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace XXF.Db
{
    class DbConnSql : DbConn
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

        /// <summary>参数类型转化</summary>
        /// <param name="Par"></param>
        /// <returns></returns>
        private SqlParameter ParameterTransform(ProcedureParameter Par)
        {
            /*车毅修改 支持无类型参数*/
            if (Par.ParType == ProcParType.Default)
                return new SqlParameter(Par.Name, Par.Value);
            SqlParameter p = new SqlParameter();
            p.ParameterName = Par.Name;
            switch (Par.ParType)
            {
                case ProcParType.Int16:
                    p.SqlDbType = SqlDbType.SmallInt;
                    break;
                case ProcParType.Int32:
                    p.SqlDbType = SqlDbType.Int;
                    break;
                case ProcParType.Int64:
                    p.SqlDbType = SqlDbType.BigInt;
                    break;
                case ProcParType.Single:
                    p.SqlDbType = SqlDbType.Real;
                    break;
                case ProcParType.Double:
                    p.SqlDbType = SqlDbType.Float;
                    break;
                case ProcParType.Decimal:
                    p.SqlDbType = SqlDbType.Decimal;
                    break;
                case ProcParType.Char:
                    p.SqlDbType = SqlDbType.Char;
                    break;
                case ProcParType.VarChar:
                    p.SqlDbType = SqlDbType.VarChar;
                    break;
                case ProcParType.NVarchar:
                    p.SqlDbType = SqlDbType.NVarChar;
                    break;
                case ProcParType.Image:
                    p.SqlDbType = SqlDbType.Binary;
                    break;
                case ProcParType.DateTime:
                    p.SqlDbType = SqlDbType.DateTime;
                    break;
                default:
                    throw new Exception("未知类型ProcParType：" + Par.ParType.ToString());
            }
            p.Size = Par.Size;
            p.Direction = Par.Direction;
            switch (Par.Direction)
            {
                case ParameterDirection.Input:
                case ParameterDirection.InputOutput:
                    if (Par.Value == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    else
                    {
                        p.Value = Par.Value;
                    }
                    break;
            }
            return p;
        }

        public override int ExecuteSql(string Sql, CommandType CmdType, List<ProcedureParameter> ProcedurePar)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = privateconn;
            cmd.Transaction = ts;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CmdType;
            cmd.CommandText = Sql;
            if (ProcedurePar != null)
            {
                for (int i = 0; i < ProcedurePar.Count; i++)
                {
                    SqlParameter p = ParameterTransform(ProcedurePar[i]);
                    cmd.Parameters.Add(p);
                }
            }
            return cmd.ExecuteNonQuery();
        }

        public override object ExecuteScalar(string Sql, CommandType CmdType, List<ProcedureParameter> ProcedurePar)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = privateconn;
            cmd.Transaction = ts;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CmdType;
            cmd.CommandText = Sql;
            if (ProcedurePar != null)
            {
                for (int i = 0; i < ProcedurePar.Count; i++)
                {
                    SqlParameter p = ParameterTransform(ProcedurePar[i]);
                    cmd.Parameters.Add(p);
                }
            }
            return cmd.ExecuteScalar();
        }

        //publClass1.csic override int ExecuteSql(string Sql, List<ImageParameter> ImagePar)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = privateconn;
        //    cmd.Transaction = ts;
        //    cmd.CommandTimeout = 0;
        //    cmd.CommandType = CommandType.Text;
        //    cmd.CommandText = Sql;
        //    for (int i = 0; i < ImagePar.Count; i++)
        //    {
        //        SqlParameter p = new SqlParameter(ImagePar[i].Name, SqlDbType.VarBinary, ImagePar[i].Value.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, ImagePar[i].Value);
        //        cmd.Parameters.Add(p);
        //    }
        //    return cmd.ExecuteNonQuery();
        //}

        public override int ExecuteProcedure(string ProcedureName, List<ProcedureParameter> ProcedurePar)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = privateconn;
            cmd.Transaction = ts;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedureName;
            for (int i = 0; i < ProcedurePar.Count; i++)
            {
                SqlParameter p = ParameterTransform(ProcedurePar[i]);
                cmd.Parameters.Add(p);
            }
            int result = cmd.ExecuteNonQuery();
            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                switch (cmd.Parameters[i].Direction)
                {
                    case ParameterDirection.InputOutput:
                    case ParameterDirection.Output:
                    case ParameterDirection.ReturnValue:
                        ProcedurePar[i] = new ProcedureParameter(ProcedurePar[i].Name, ProcedurePar[i].ParType, ProcedurePar[i].Size, ProcedurePar[i].Direction, cmd.Parameters[i].Value);
                        break;
                }
            }
            return result;
        }

        public override DbDataReader SqlToDbReader(string Sql, List<ProcedureParameter> ProcedurePar)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = privateconn;
            cmd.Transaction = ts;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Sql;
            if (ProcedurePar != null)
            {
                for (int i = 0; i < ProcedurePar.Count; i++)
                {
                    SqlParameter p = ParameterTransform(ProcedurePar[i]);
                    cmd.Parameters.Add(p);
                }
            }
            return cmd.ExecuteReader();
        }

        public override int GetIdentity()
        {
            DataSet ds = new DataSet();
            SqlToDataSet(ds, "select @@identity", null);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return 0;
            }
            else
            {
                return LibConvert.ObjToInt(ds.Tables[0].Rows[0][0]);
            }
        }

        public override bool FieldIsExist(string aTableName, string aFieldName)
        {
            string TempSql = "select top 0 * from " + aTableName;
            DataSet ds = new DataSet();
            SqlToDataSet(ds, TempSql, null);
            if (ds.Tables[0].Columns.IndexOf(aFieldName) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool FieldIsExist(string aDbName, string aTableName, string aFieldName)
        {
            string TempSql = "select top 0 * from " + aDbName + ".." + aTableName;
            DataSet ds = new DataSet();
            SqlToDataSet(ds, TempSql, null);
            if (ds.Tables[0].Columns.IndexOf(aFieldName) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override DateTime GetServerDate()
        {
            DateTime Result = new DateTime(0);
            SqlCommand cmdSql = new SqlCommand("select GetDate() as aDate", privateconn);
            cmdSql.Transaction = ts;
            SqlDataReader DrSql = cmdSql.ExecuteReader();
            if (DrSql.Read())
                Result = DrSql.GetDateTime(0);
            DrSql.Close();
            return Result;
        }

        public override void SqlToDataSet(DataSet ds, string Sql, List<ProcedureParameter> ProcedurePar)
        {
            SqlCommand selectCmd = new SqlCommand();
            selectCmd.CommandTimeout = 0;
            selectCmd.Transaction = ts;
            selectCmd.Connection = privateconn;
            selectCmd.CommandType = CommandType.Text;
            selectCmd.CommandText = Sql;
            if (ProcedurePar != null)
            {
                for (int i = 0; i < ProcedurePar.Count; i++)
                {
                    SqlParameter p = ParameterTransform(ProcedurePar[i]);
                    selectCmd.Parameters.Add(p);
                }
            }
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = selectCmd;
            da.Fill(ds);
        }

        public override void SqlBulkCopy(DataTable dt, string datatablename, string Sql, List<ProcedureParameter> ProcedurePar, int BatchSize = 0)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(privateconn, SqlBulkCopyOptions.Default, ts);
            sqlBulkCopy.DestinationTableName = datatablename;
            if (BatchSize > 0)
            {
                sqlBulkCopy.BatchSize = BatchSize;
            }
            sqlBulkCopy.WriteToServer(dt);
            sqlBulkCopy.Close();
        }

        public override void SqlBulkCopy(DataTable dt, string datatablename, string Sql, List<ProcedureParameter> ProcedurePar, Dictionary<string, string> mapps, int BatchSize = 0)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(privateconn, SqlBulkCopyOptions.Default, ts);
            sqlBulkCopy.DestinationTableName = datatablename;
            foreach (var mapp in mapps)
            {
                sqlBulkCopy.ColumnMappings.Add(mapp.Key, mapp.Value);
            }
            if (BatchSize > 0)
            {
                sqlBulkCopy.BatchSize = BatchSize;
            }
            sqlBulkCopy.WriteToServer(dt);
            sqlBulkCopy.Close();
        }

        public override void SqlToDataSet(DataSet ds, string Sql, List<ProcedureParameter> ProcedurePar, string TableName)
        {
            SqlCommand selectCmd = new SqlCommand();
            selectCmd.CommandTimeout = 0;
            selectCmd.Transaction = ts;
            selectCmd.Connection = privateconn;
            selectCmd.CommandType = CommandType.Text;
            selectCmd.CommandText = Sql;
            if (ProcedurePar != null)
            {
                for (int i = 0; i < ProcedurePar.Count; i++)
                {
                    SqlParameter p = ParameterTransform(ProcedurePar[i]);
                    selectCmd.Parameters.Add(p);
                }
            }
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = selectCmd;
            da.Fill(ds, TableName);
        }

        public override bool TableIsExist(string aTableName)
        {
            DataSet Ds = new DataSet();
            SqlToDataSet(Ds, "Select name from sysobjects where Name='" + aTableName + "'", null);
            if (Ds.Tables[0].Rows.Count == 0)
                return false;
            else
                return true;
        }

        public override bool TableIsExist(string aDbName, string aTableName)
        {
            DataSet Ds = new DataSet();
            SqlToDataSet(Ds, "Select name from " + aDbName + "..sysobjects where Name='" + aTableName + "'", null);
            if (Ds.Tables[0].Rows.Count == 0)
                return false;
            else
                return true;
        }

        private static long _tick = DateTime.Now.Ticks;

        public override string GetMcsToSql(string AMultiChoiceStr, string aField, int aStyle, ref List<Db.ProcedureParameter> aPar)
        {
            if (aPar == null) aPar = new List<ProcedureParameter>();
            if (AMultiChoiceStr == "") return "1=1";
            string[] SqlSz = AMultiChoiceStr.Split(",，".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string sRet = "";
            for (int i = 0; i < SqlSz.Length; i++)
            {
                string TmpStr = "";
                int pos = SqlSz[i].IndexOf("|");
                if (pos == -1)
                {
                    long _t = _tick;
                    _tick++;
                    aPar.Add(new Db.ProcedureParameter("@find_sql_" + _t.ToString(), Db.ProcParType.VarChar, 255, SqlSz[i]));
                    if (aStyle == 0)
                        TmpStr = aField + " Like @find_sql_" + _t.ToString();
                    else if (aStyle == 1)
                        TmpStr = aField + " Like '%'+@find_sql_" + _t.ToString();
                    else if (aStyle == 2)
                        TmpStr = aField + " Like @find_sql_" + _t.ToString() + "+'%'";
                    else if (aStyle == 3)
                        TmpStr = aField + " Like '%'+@find_sql_" + _t.ToString() + "+'%'";
                }
                else
                {
                    long _t1 = _tick;
                    _tick++;
                    long _t2 = _tick;
                    _tick++;
                    string str1 = SqlSz[i].Substring(0, pos);
                    string str2 = SqlSz[i].Substring(pos + 1, SqlSz[i].Length - pos - 1);
                    aPar.Add(new Db.ProcedureParameter("@find_sql_" + _t1.ToString(), Db.ProcParType.VarChar, 255, str1));
                    TmpStr = "(left(" + aField + "," + str1.Length + ")>=@find_sql_" + _t1.ToString();
                    aPar.Add(new Db.ProcedureParameter("@find_sql_" + _t2.ToString(), Db.ProcParType.VarChar, 255, str2));
                    TmpStr = TmpStr + " and left(" + aField + "," + str2.Length + ")<=@find_sql_" + _t2.ToString() + ")";
                }
                if (sRet == "")
                    sRet = TmpStr;
                else
                    sRet = sRet + " or " + TmpStr;
            }
            return "(" + sRet + ")";
        }
    }
}
