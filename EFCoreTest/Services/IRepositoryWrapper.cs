using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Services
{
    public interface IRepositoryWrapper
    {
        IAuthorRepository Author { get; }
        IBookRepository Book { get; }
    }
}
