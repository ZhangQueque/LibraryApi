using AutoMapper;
using EFCoreTest.Entities;
using EFCoreTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Helpes
{
    /// <summary>
    /// 映射配置文件
    /// </summary>
    public class LibraryMappingProfile : Profile
    { 
        public LibraryMappingProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<Author, AuthorDto>();
            CreateMap<AuthorDto, Author>();
            CreateMap<BookDto, Book>();
            CreateMap<UpdateAuthorDto, Author>();
            CreateMap<UpdateBookDto, Book>();
        }
    }
}
