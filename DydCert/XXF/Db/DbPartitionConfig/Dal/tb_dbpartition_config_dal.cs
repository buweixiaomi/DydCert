using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XXF.Extensions;
using XXF.Db;
using XXF.Db.DbPartitionConfig.Model;

namespace XXF.Db.DbPartitionConfig.Dal
{

	public partial class tb_dbpartition_config_dal
    {

        public virtual tb_dbpartition_config_model Get(DbConn PubConn, int id)
        {
            List<ProcedureParameter> Par = new List<ProcedureParameter>();
            Par.Add(new ProcedureParameter("@id", id));
            StringBuilder stringSql = new StringBuilder();
            stringSql.Append(@"select s.* from tb_dbpartition_config s where s.id=@id");
            int rev = PubConn.ExecuteSql(stringSql.ToString(), Par);
            DataSet ds = new DataSet();
            PubConn.SqlToDataSet(ds, stringSql.ToString(), Par);
            if (ds != null && ds.Tables.Count > 0)
            {
				return CreateModel(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        public virtual List<tb_dbpartition_config_model> Get(DbConn PubConn,DateTime lastUpdateTime)
        {
            return XXF.ProjectTool.SqlHelper.Visit(ps =>
             {

                 List<ProcedureParameter> Par = new List<ProcedureParameter>();
                 StringBuilder stringSql = new StringBuilder();
                 if (lastUpdateTime == default(DateTime))
                 {
                     stringSql.Append(@"select s.* from tb_dbpartition_config s ");
                 }
                 else
                 {
                     stringSql.Append(@"select s.* from tb_dbpartition_config s  where s.f_last_update_time>=@lastTime");
                     ps.Add("lastTime", lastUpdateTime);
                 }
             //    int rev = PubConn.ExecuteSql(stringSql.ToString(), ps.ToParameters());
                 DataSet ds = new DataSet();
                 PubConn.SqlToDataSet(ds, stringSql.ToString(), ps.ToParameters());
                 var rs = new List<tb_dbpartition_config_model>();
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

		public virtual tb_dbpartition_config_model CreateModel(DataRow dr)
        {
            return new tb_dbpartition_config_model
            {
				
				//����
				id = dr["id"].Toint(),
				//����id��������ʶ�ţ�
				partitionno = dr["f_partitionno"].Toint(),
				//��������(1:�û�����,2:�̻������)
				partitiontype = dr["f_partitiontype"].ToByte(),
				//��������ַ
				dbserver = dr["f_dbserver"].Tostring(),
				//���ݿ���
				dbname = dr["f_dbname"].Tostring(),
				//�û���
				dbuser = dr["f_dbuser"].Tostring(),
				//����
				dbpass = dr["f_dbpass"].Tostring(),

                lastUpdateTime = LibConvert.ObjToDateTime(dr["f_last_update_time"]),

                //�Ƿ�ɾ��

                isDel = LibConvert.ObjToBool(dr["f_isdel"])

            };
        }
    }
}