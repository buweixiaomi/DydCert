using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XXF.Db
{
    /// <summary>账期类,其实就是YYYYMM格式的日期</summary>
    public class FinaceDate
    {
        #region 属性
        private DateTime date = new DateTime();

        /// <summary>获取或设置此实例的年份</summary>
        public int Year
        {
            get { return date.Year; }
            set { date = new DateTime(value, date.Month, 1); }
        }

        /// <summary>获取或设置此实例的月份</summary>
        public int Month
        {
            get { return date.Month; }
            set { date = new DateTime(date.Year, value, 1); }
        }

        /// <summary>获取账期字符串YYYYMM格式</summary>
        public string Value
        {
            get { return date.ToString("yyyyMM"); }
        }

        /// <summary>获取账期字符串[yyyy年MM月]格式</summary>
        public string CnValue
        {
            get { return date.ToString("yyyy年MM月"); }
        }
        #endregion 属性

        #region 构造函数
        /// <summary>构造函数，参数必须为YYYYMM格式</summary>
        /// <param name="aStr"></param>
        public FinaceDate(string aStr)
        {
            if (aStr.Length != 6) throw new Exception("FinaceDate类构造函数参数格式非法，必须为yyyyMM格式！");
            date = new DateTime(LibConvert.StrToInt(aStr.Substring(0, 4)), LibConvert.StrToInt(aStr.Substring(4, 2)), 1, 0, 0, 0);
        }

        /// <summary>构造函数</summary>
        /// <param name="aDate"></param>
        public FinaceDate(DateTime aDate)
        {
            date = aDate;
        }

        /// <summary>构造函数</summary>
        /// <param name="aYear">年</param>
        /// <param name="aMonth">月</param>
        public FinaceDate(int aYear, int aMonth)
        {
            date = new DateTime(aYear, aMonth, 1);
        }
        #endregion 构造函数

        #region 方法
        /// <summary>将指定的月份加到此实例上</summary>
        /// <param name="months">月份数，months参数可以是正数也可以是负数</param>
        public void AddMonth(int months)
        {
            date = date.AddMonths(months);
        }

        /// <summary>判断参数中的实例和当前实例是否相等</summary>
        /// <param name="aDate"></param>
        /// <returns></returns>
        public bool Equal(FinaceDate aDate)
        {
            return (date.Year == aDate.Year && date.Month == aDate.Month);
        }

        /// <summary>将参数中值与此实例做比较，实例大返回正数，参数大返回负数</summary>
        /// <param name="aDate">用来做比较的对象</param>
        /// <returns>实例大返回正数，参数大返回负数</returns>
        public int Compare(FinaceDate aDate)
        {
            return (date.Year - aDate.Year) * 12 + date.Month - aDate.Month;
        }
        /// <summary>将参数中值与此实例做比较，实例大返回正数，参数大返回负数</summary>
        /// <param name="aDate">用来做比较的对象</param>
        /// <returns>实例大返回正数，参数大返回负数</returns>
        public int Compare(DateTime aDate)
        {
            return (date.Year - aDate.Year) * 12 + date.Month - aDate.Month;
        }
        #endregion 方法
    }
}
