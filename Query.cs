using System;
using System.Collections.Generic;
using System.Text;

namespace BookExploreProxy
{

    public class Query
    {
        // What the query is for e.g login or register
        public string PayloadType { get; set; }
        // Data required to perform query e.g email, password
        public string[] Payload { get; set; }

    }

}
