using EFCoreTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Services
{
    /// <summary>
    /// 仓储包装
    /// </summary>
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly LibraryDbContext libraryDbContext;

        public IAuthorRepository Author => new AuthorRepository(libraryDbContext);

        public IBookRepository Book => new BookRepository(libraryDbContext);



        public RepositoryWrapper(LibraryDbContext libraryDbContext)
        {
            this.libraryDbContext = libraryDbContext;
        }


    }
}
