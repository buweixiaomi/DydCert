using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XXF.Extensions;
using XXF.Db;
using XXF.Db.DbSubscribeRuleConfig.Model;

namespace XXF.Db.DbSubscribeRuleConfig.Dal
{
	/*代码自动生成工具自动生成,不要在这里写自己的代码，否则会被自动覆盖哦 - 车毅*/
	public partial class tb_dbsubscribe_config_dal
    {
        public virtual bool Add(DbConn PubConn, tb_dbsubscribe_config_model model)
        {

            List<ProcedureParameter> Par = new List<ProcedureParameter>()
                {
					
					//
					new ProcedureParameter("@f_partitionno",    model.partitionno),
					//
					new ProcedureParameter("@f_partitiontype",    model.partitiontype),
					//
					new ProcedureParameter("@f_dbserver",    model.dbserver),
					//
					new ProcedureParameter("@f_dbname",    model.dbname),
					//
					new ProcedureParameter("@f_dbuser",    model.dbuser),
					//
					new ProcedureParameter("@f_dbpass",    model.dbpass),
					//
					new ProcedureParameter("@f_dbtype",    model.dbtype)   
                };
            int rev = PubConn.ExecuteSql(@"insert into tb_dbsubscribe_config(f_partitionno,f_partitiontype,f_dbserver,f_dbname,f_dbuser,f_dbpass,f_dbtype)
										   values(@f_partitionno,@f_partitiontype,@f_dbserver,@f_dbname,@f_dbuser,@f_dbpass,@f_dbtype)", Par);
            return rev == 1;

        }

        public virtual bool Edit(DbConn PubConn, tb_dbsubscribe_config_model model)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>()
            {
                    
					//
					new ProcedureParameter("@f_partitionno",    model.partitionno),
					//
					new ProcedureParameter("@f_partitiontype",    model.partitiontype),
					//
					new ProcedureParameter("@f_dbserver",    model.dbserver),
					//
					new ProcedureParameter("@f_dbname",    model.dbname),
					//
					new ProcedureParameter("@f_dbuser",    model.dbuser),
					//
					new ProcedureParameter("@f_dbpass",    model.dbpass),
					//
					new ProcedureParameter("@f_dbtype",    model.dbtype)
            };
			Par.Add(new ProcedureParameter("@f_id",  model.id));

            int rev = PubConn.ExecuteSql("update tb_dbsubscribe_config set f_partitionno=@f_partitionno,f_partitiontype=@f_partitiontype,f_dbserver=@f_dbserver,f_dbname=@f_dbname,f_dbuser=@f_dbuser,f_dbpass=@f_dbpass,f_dbtype=@f_dbtype where f_id=@f_id", Par);
            return rev == 1;

        }

        public virtual bool Delete(DbConn PubConn, int f_id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@f_id",  f_id));

            string Sql = "delete from tb_dbsubscribe_config where f_id=@f_id";
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

        public virtual List<tb_dbsubscribe_config_model> Get(DbConn PubConn, DateTime lastUpdateTime)
        {
            return XXF.ProjectTool.SqlHelper.Visit(ps =>
            {
                List<ProcedureParameter> Par = new List<ProcedureParameter>();
                StringBuilder stringSql = new StringBuilder();
                if (lastUpdateTime == default(DateTime))
                {
                    stringSql.Append(@"select s.* from tb_dbsubscribe_config s ");
                }
                else
                {
                    stringSql.Append(@"select s.* from tb_dbsubscribe_config s  where s.f_last_update_time>=@lastTime");
                    ps.Add("lastTime", lastUpdateTime);
                }

               // int rev = PubConn.ExecuteSql(stringSql.ToString(), ps.ToParameters());
                DataSet ds = new DataSet();
                PubConn.SqlToDataSet(ds, stringSql.ToString(), ps.ToParameters());
                var rs = new List<tb_dbsubscribe_config_model>();
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var r = CreateModel(dr);
                        rs.Add(r);
                    }
                }
                return rs;
            });
        }

		public virtual tb_dbsubscribe_config_model CreateModel(DataRow dr)
        {
            var o = new tb_dbsubscribe_config_model();
			
			//
			if(dr.Table.Columns.Contains("f_id"))
			{
				o.id = dr["f_id"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("f_partitionno"))
			{
				o.partitionno = dr["f_partitionno"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("f_partitiontype"))
			{
				o.partitiontype = dr["f_partitiontype"].Toint();
			}
			//
			if(dr.Table.Columns.Contains("f_dbserver"))
			{
				o.dbserver = dr["f_dbserver"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("f_dbname"))
			{
				o.dbname = dr["f_dbname"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("f_dbuser"))
			{
				o.dbuser = dr["f_dbuser"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("f_dbpass"))
			{
				o.dbpass = dr["f_dbpass"].Tostring();
			}
			//
			if(dr.Table.Columns.Contains("f_dbtype"))
			{
				o.dbtype = dr["f_dbtype"].Toint();
			}

            //最后更新时间
            if (dr.Table.Columns.Contains("f_last_update_time"))
            {
                o.lastUpdateTime = LibConvert.ObjToDateTime(dr["f_last_update_time"]);
            }
            //是否删除
            if (dr.Table.Columns.Contains("f_isdel"))
            {
                o.isDel = LibConvert.ObjToBool(dr["f_isdel"]);
            }
			return o;
        }
    }
}