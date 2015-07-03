using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertSdk.newsdk
{
    public class TwoNodeChain : ISdk
    {
        public Token Get(string token)
        {
            throw new NotImplementedException();
        }
        private static TwoNodeChain Instance = new TwoNodeChain();

        private int _currentlength = 0;

        static TwoNodeChain() { }
        private TwoNodeChain() { }

        public static TwoNodeChain GetInstance()
        {
            return Instance;
        }
        public void Add(Token t)
        {
            throw new NotImplementedException();
        }

        public List<Token> GetTokens()
        {
            throw new NotImplementedException();
        }

        public int GetLength()
        {
            throw new NotImplementedException();
        }

        public void WriteTopToFile(int topcount)
        {
            throw new NotImplementedException();
        }

        private class TNode
        {
            public TNode ParentNode { get; set; }
            public TNode LeftNode { get; set; }
            public TNode RightNode { get; set; }
            public int NodeId { get; set; }
            public bool Used { get; set; }
            public int AccessCount { get; set; }
            public Token Token { get; set; }
        }
    }
}
