using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Fix.Common
{
    public class group
    {
        public string id { get; set; }
        public string text { get; set; }
        public string url { get; set; }

        public item item { get; set; }
    }
    public class item
    {
        public string id { get; set; }
        public string text { get; set; }
        public string url { get; set; }
    }
}
