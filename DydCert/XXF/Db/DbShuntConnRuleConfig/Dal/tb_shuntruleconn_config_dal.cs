using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XXF.Extensions;
using XXF.Db;
using XXF.Db.DbShuntRuleConnConfig.Model;

namespace XXF.Db.DbShuntRuleConnConfig.Dal
{

	public partial class tb_shuntruleconn_config_dal
    {
        public virtual tb_shuntruleconn_config_model Get(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", id));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_distribution_rule_config s where s.id=@id");
            int rev = PubConn.ExecuteSql(stringSql.ToString(), Par);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0)
            {
				return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        public virtual List<tb_shuntruleconn_config_model> Get(DbConn PubConn,DateTime lastUpdateTime)
        {
            return XXF.ProjectTool.SqlHelper.Visit(ps =>
            {
                List<ProcedureParameter> Par = new List<ProcedureParameter>();
                StringBuilder stringSql = new StringBuilder();
                if (lastUpdateTime == default(DateTime))
                {
                    stringSql.Append(@"select s.* from tb_distribution_rule_config s");
                }
                else
                {
                    stringSql.Append(@"select s.* from tb_distribution_rule_config s where s.f_last_update_time>=@lastTime");
                    ps.Add("lastTime", lastUpdateTime);
                }
          //      int rev = PubConn.ExecuteSql(stringSql.ToString(), ps.ToParameters());
                DataSet ds = new DataSet();
                PubConn.SqlToDataSet(ds, stringSql.ToString(), ps.ToParameters());
                var rs = new List<tb_shuntruleconn_config_model>();
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

        public virtual tb_shuntruleconn_config_model CreateModel(DataRow dr)
        {
            return new tb_shuntruleconn_config_model
            {
                //自增
                id = dr["id"].Toint(),
                //分区id（分区标识号）
                partitionno = dr["f_partitionno"].Toint(),
                //地区编码
                regioncode = dr["f_regioncode"].Toint(),
                //服务器地址
                dbserver = dr["f_dbserver"].Tostring(),
                //数据库名
                dbname = dr["f_dbname"].Tostring(),
                //用户名
                dbuser = dr["f_dbuser"].Tostring(),
                //密码
                dbpass = dr["f_dbpass"].Tostring(),
                //最后更新时间

                lastUpdateTime = LibConvert.ObjToDateTime(dr["f_last_update_time"]),

                //是否删除

                isDel = LibConvert.ObjToBool(dr["f_isdel"])

            };
        }
    }
}