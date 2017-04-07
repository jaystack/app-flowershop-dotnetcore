using System.Collections.Generic;
using SystemEndpointsDotnetCore.Models;

namespace App.Flowershop
{
    public class Config
    {
        public AppKeyValuePair Apps { get; set; }

        public List<Endpoint> hosts { get; set; } = new List<Endpoint>();
    }

    public class AppKeyValuePair
    {
        public string Items { get; set; }

        public string Cart { get; set; }  
    }
}
