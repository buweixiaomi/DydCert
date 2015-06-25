using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertCenter.Models
{
    public class Z_TreeModel
    {
        public string id { get; set; }

        public string name { get; set; }

        public string isParent { get; set; }

        public string halfCheck { get; set; }

        public string @checked { get; set; }

        public string iconSkin { get; set; }
        public string open { get; set; }
        public List<Z_TreeModel> children { get; set; }
    }
}
