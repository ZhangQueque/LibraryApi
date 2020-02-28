using EFCoreTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Services
{
    public interface IBookRepository : IRepositoryBase<Book>, IRepositoryBase2<Book, int>
    {
        Task<IEnumerable<Book>> GetBooksAsync(int authorId);
        Task<Book> GetBookAsync(int authorId, int boolId);
    }
}
