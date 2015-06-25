using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertCenter.Models.DbModels
{
    public partial class appcategory
    {
        public int apptype { get; set; }

        public int categoryid { get; set; }

        public string categorytitle { get; set; }

        public string categorydesc { get; set; }

    }
}