using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XXF.Api
{
    /// <summary>
    /// 基础request参数类型
    /// </summary>
    public abstract class ParmField
    {
        protected string _key;
        protected string _value;
        /// <summary>
        /// 参数key
        /// </summary>
        public string Key
        {
            get { return _key; }
        }
        /// <summary>
        /// 参数value
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        protected Stream _streamvalue;
        /// <summary>
        /// 参数流内容
        /// </summary>
        public Stream StreamValue
        { get { return _streamvalue; } }
    }
    /// <summary>
    /// 字符串参数类型
    /// </summary>
    public class StringField : ParmField
    {
        public StringField(string key, object value)
        {
            _key = key;
            if (value == null)
            {
                _value = "";
            }
            else
            {
                _value = value.ToString();
            }
        }
    }
    /// <summary>
    /// 文件参数类型
    /// </summary>
    public class FileField : ParmField
    {
        public FileField(string key, Stream streamvalue, string filename)
        {
            _key = key;
            _streamvalue = streamvalue;
            _value = filename;
        }
    }
    /// <summary>
    /// 数据流参数类型
    /// </summary>
    public class StreamField : ParmField
    {
        public StreamField(string key, Stream stream)
        {
            _key = key;
            _streamvalue = stream;
        }
    }
}
