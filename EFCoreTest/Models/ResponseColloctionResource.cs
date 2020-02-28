using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Models
{
    public class ResponseColloctionResource<T> : ReponseReSource
    {
        public ResponseColloctionResource(List<T> items)
        {
            Items = items;
        }
        public List<T> Items { get; }
    }
}
