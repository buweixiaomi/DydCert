using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk
{
    public interface ISdk
    {
        Token Get(string token);
        void Add(Token t);
        List<Token> GetTokens();
        int GetLength();

        void WriteTopToFile(int topcount);
    }
}
