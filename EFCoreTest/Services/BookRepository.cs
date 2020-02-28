using EFCoreTest.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Services
{
    public class BookRepository : RepositoryBase<Book, int>, IBookRepository
    {
        public BookRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public async Task<Book> GetBookAsync(int authorId, int boolId)
        {
            return await DbContext.Set<Book>().FirstOrDefaultAsync(m => m.AuthorId == authorId && m.BookId == boolId);
        }

        public Task<IEnumerable<Book>> GetBooksAsync(int authorId)
        {
            return Task.FromResult(DbContext.Set<Book>().Where(m => m.AuthorId == authorId).AsEnumerable());
        }
    }
}
