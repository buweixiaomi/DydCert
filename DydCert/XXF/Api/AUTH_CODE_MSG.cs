using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXF.Api
{
    /// <summary>
    /// 用户认证模块错误码（交互协议）类
    /// </summary>
    public class AUTH_CODE_MSG
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public static Dictionary<int, string> msgs = new Dictionary<int, string>();

        static AUTH_CODE_MSG()
        {
            msgs.Add(-100, "认证失败");
            msgs.Add(-905, "token不存在或已过期");
            msgs.Add(-102, "请求超时");
            msgs.Add(-103, "appid不存在");
            msgs.Add(-104, "	sign不正确");
            msgs.Add(-105, "无权操作该接口");
            msgs.Add(-106, "接口已被冻结");
            msgs.Add(-107, "应用被冻结");
            msgs.Add(-108, "接口不存在");

            msgs.Add(-111, "参数不完整");
            msgs.Add(-112, "用户不存在");
            msgs.Add(-113, "pwd不正确");
            msgs.Add(-114, "用户被冻结");
        }
        /// <summary>
        /// 根据错误码获取详细错误信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string Get(int index)
        {
            if (msgs.ContainsKey(index))
                return msgs[index];
            else
            {
                if (index > 0)
                {
                    return "success";
                }
                else
                {
                    return "faild";
                }
            }
        }
    }
}
