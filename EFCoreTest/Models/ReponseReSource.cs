using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Models
{
    public class ReponseReSource
    {
        [JsonProperty("_links", Order = 100)]
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
