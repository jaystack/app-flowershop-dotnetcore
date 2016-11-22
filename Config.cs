using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Flowershop
{
    public class Config
    {
        public AppKeyValuePair Apps { get; set; }
    }

    public class AppKeyValuePair
    {
        public string Items { get; set; }

        public string Cart { get; set; }
    }
}
