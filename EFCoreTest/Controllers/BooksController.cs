using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EFCoreTest.Attributes;
using EFCoreTest.Entities;
using EFCoreTest.Models;
using EFCoreTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EFCoreTest.Controllers
{
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    [ServiceFilter(typeof(IsAuthorExistAttribute))]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public BooksController(IRepositoryWrapper repositoryWrapper, IMapper mapper, IMemoryCache memoryCache)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }
        [HttpGet(Name = nameof(GetBooksAsync))]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksAsync(int authorId)
        {
            var bookDtos = new List<BookDto>();
            string key = $"{authorId}_bok";
            if (!memoryCache.TryGetValue(key, out bookDtos))
            {
                var books = await repositoryWrapper.Book.GetBooksAsync(authorId);
                bookDtos = mapper.Map<IEnumerable<BookDto>>(books).ToList();
                memoryCache.Set(key, bookDtos);
            }

            return bookDtos;
        }
        /// <summary>
        /// 根据id获取，别忘了nameof
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpGet("{bookId}", Name = nameof(GetBookAsync))]
        public async Task<ActionResult<BookDto>> GetBookAsync(int authorId, int bookId)
        {
            var book = await repositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }
            return mapper.Map<BookDto>(book);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBookAsybc(int authorId, UpdateBookDto bookDto)
        {
            var book = mapper.Map<Book>(bookDto);
            book.AuthorId = authorId;
            repositoryWrapper.Book.Create(book);
            if (await repositoryWrapper.Book.SaveAsync())
            {
                var bookCreate = mapper.Map<BookDto>(book);
                return CreatedAtRoute(nameof(GetBookAsync), new { authorId = bookCreate.AuthorId, bookId = bookCreate.BookId }, bookCreate);
            }
            return BadRequest();
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBoolAsync(int authorId, int bookId)
        {
            var book = await repositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }
            repositoryWrapper.Book.Delete(book);
            if (!await repositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("删除资源失败");
            }
            return NoContent();
        }
        [HttpPut("{bookId}")] 
        public async Task<IActionResult> UpdateBookAsync(int authorId, int bookId, UpdateBookDto bookDto)
        {
            var book = await repositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }
            mapper.Map(bookDto, book);
            repositoryWrapper.Book.Update(book);
            if (!await repositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("Edit Bool have a erro");
            }
            return NoContent();
        }
    }
}
