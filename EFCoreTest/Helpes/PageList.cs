using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Helpes
{
    public class PageList<T> : List<T>
    {
        public PageList(IQueryable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
            AddRange(items);
        }

        public int TotalCount { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public bool HasPrevious => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPages;

        public static async Task<PageList<T>> CreatePageLsit(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var totalCount = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var list = new PageList<T>(items, totalCount, pageIndex, pageSize);
            return await Task.FromResult(list);
        }

    }
}
