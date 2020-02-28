using EFCoreTest.Entities;
using EFCoreTest.Helpes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
namespace EFCoreTest.Services
{
    public class AuthorRepository : RepositoryBase<Author, int>, IAuthorRepository
    {
        public AuthorRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public Task<PageList<Author>> GetPageAsync(ResourcesParameters pageParameters)
        {
            IQueryable<Author> source = DbContext.Set<Author>();

            if (!string.IsNullOrEmpty(pageParameters.AuthorName))
            {
                source = source.Where(m => m.Name.Contains(pageParameters.AuthorName));
            }
            var orderSource = source.OrderBy(pageParameters.SortBy);
            return PageList<Author>.CreatePageLsit(orderSource, pageParameters.PageIndex, pageParameters.PageSize);
        }

        public IEnumerable<Author> Test()
        {
            var authors = DbContext.Set<Author>().Where(m => m.Id < 10 && m.Name.Contains("张")).Include(m => m.Books);

            return authors;
        }
    }
}
