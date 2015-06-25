using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CertComm
{
    public abstract class ParmField
    {
        protected string _key;
        protected string _value;
        public string Key
        {
            get { return _key; }
        }
        public string Value
        {
            get { return _value; }
        }

        protected Stream _streamvalue;
        public Stream StreamValue
        { get { return _streamvalue; } }
    }

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
    public class FileField : ParmField
    {
        public FileField(string key, Stream streamvalue, string filename)
        {
            _key = key;
            _streamvalue = streamvalue;
            _value = filename;
        }
    }
    public class StreamField : ParmField
    {
        public StreamField(string key, Stream stream)
        {
            _key = key;
            _streamvalue = stream;
        }
    }
}
