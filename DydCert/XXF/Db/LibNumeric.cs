using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXF.Db
{
    public static class LibNumeric
    {
        /// <summary>【通用函数】取随机数字
        /// </summary>
        /// <param name="aMin">最小</param>
        /// <param name="aMax">最大</param>
        public static int GetRandomNumeric(int aMin, int aMax)
        {
            int num = 0;
            Random ro = new Random(unchecked((int)DateTime.Now.Ticks));
            num = ro.Next(aMin, aMax);
            return num;
        }
    }
}
