using EFCoreTest.Entities;
using EFCoreTest.Helpes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Services
{
    public interface IAuthorRepository : IRepositoryBase<Author>, IRepositoryBase2<Author, int>
    {
        IEnumerable<Author> Test();

        Task<PageList<Author>> GetPageAsync(ResourcesParameters pageParameters);
    }
}
