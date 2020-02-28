using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Models
{
    public class Link
    {
        public Link(string method, string rel, string href)
        {
            Method = method;
            Rel = rel;
            Href = href;
        }

        public string Method { get; }
        public string Rel { get; }
        public string Href { get; }
    }
}
