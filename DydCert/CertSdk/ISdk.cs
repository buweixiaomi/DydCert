using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk
{
    public class ISdk
    {
        public Token Get(string token);
        public void Add(Token t);
    }
}
