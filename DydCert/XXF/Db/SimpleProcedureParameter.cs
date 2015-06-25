using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXF.Db
{
    /// <summary>
    /// 简易参数写法 车毅
    /// 简化并扩展原先的参数书写方式
    /// </summary>
    public class SimpleProcedureParameter : Dictionary<string, object>
    {
        /// <summary>
        /// 转化成框架支持的参数数组
        /// </summary>
        /// <returns></returns>
        public List<ProcedureParameter> ToParameters()
        {
            List<Db.ProcedureParameter> Par = new List<Db.ProcedureParameter>();
            foreach (var d in this.Keys)
            {
                Par.Add(new ProcedureParameter("@" + d.TrimStart('@'), this[d]));
            }
            return Par;
        }
    }
}
