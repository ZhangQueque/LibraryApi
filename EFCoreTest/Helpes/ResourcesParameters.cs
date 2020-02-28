using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Helpes
{
    public class ResourcesParameters
    {
        private int pageSize = 3;
        private int maxPageSize = 50;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
        public int PageIndex { get; set; } = 1;

        public string AuthorName { get; set; }

        public string SortBy { get; set; } = "Id desc";
    }
}
