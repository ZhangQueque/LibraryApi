using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EFCoreTest.Entities;
using EFCoreTest.Helpes;
using EFCoreTest.Models;
using EFCoreTest.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace EFCoreTest.Controllers
{
    [Route("api/authors")]
    [ApiController]

    public class AuthorController : ControllerBase
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly IMapper mapper;
        private readonly ILogger<AuthorController> logger;

        public AuthorController(IRepositoryWrapper repositoryWrapper, IMapper mapper,
            ILogger<AuthorController> logger)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.mapper = mapper;
            this.logger = logger;
        }
        #region IEnumable延迟加载获取全部用户
        /// <summary>
        /// 获取全部作者信息
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[ResponseCache(Duration = 20)]
        //public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthorsAsync()
        //{
        //    //异步方法，await等待完后在执行后面的方法
        //    var authors = await repositoryWrapper.Author.GetAllAsync();
        //    //var a = repositoryWrapper.Author.Test();
        //    //var v = a .ToList();
        //    //var b = authors.Where(m => m.Id > 2).ToList();
        //    //使用AutoMapper映射称数据传输对象
        //    var authorHash = HashFactory.GetHash(authors);
        //    Response.Headers[HeaderNames.ETag] = authorHash;
        //    if (Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var requestHash) && authorHash == requestHash)
        //    {
        //        return StatusCode(StatusCodes.Status304NotModified);
        //    }
        //    var arthorDtos = mapper.Map<IEnumerable<AuthorDto>>(authors);

        //    return arthorDtos.ToList();
        //}
        #endregion

        #region IEnumable延迟加载分页显示(在用的时候生成sql语句（拿到全部数据），where是对内存的操作)
        //[HttpGet()]
        //public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthorsAsync([FromQuery] PageParameters pageParameters)
        //{
        //    int a = pageParameters.PageSize;
        //    int b = pageParameters.PageIndex;
        //    //异步方法，await等待完后在执行后面的方法
        //    var authors = (await repositoryWrapper.Author.GetAllAsync()).Skip((pageParameters.PageIndex - 1) * pageParameters.PageSize).Take(pageParameters.PageSize);

        //    //使用AutoMapper映射称数据传输对象
        //    var arthorDtos = mapper.Map<IEnumerable<AuthorDto>>(authors);

        //    return arthorDtos.ToList();

        //}
        #endregion

        #region IQueryable延迟加载分页显示（每用一次的时候就加载sql语句（会把where的条件加上））

        /// <summary>
        /// 获取全部作者信息
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetAuthorsAsync))]
        //缓冲中间件和Http缓冲使用一个特性，还可以针对查询关键字区分不同的响应缓冲
        [ResponseCache(Duration = 20)]//, VaryByQueryKeys = new string[] { "PageIndex", "PageSize" }
        public async Task<ActionResult<ResponseColloctionResource<AuthorDto>>> GetAuthorsAsync([FromQuery] ResourcesParameters resourcesParameters)
        {

            //获取数据
            var authors = await repositoryWrapper.Author.GetPageAsync(resourcesParameters);
            #region 验证客户端缓冲
            ////验证客户端缓冲 和缓冲中间建分开用，可以考虑和imemorycache配合使用
            //var authorHash = HashFactory.GetHash(authors);
            //Response.Headers[HeaderNames.ETag] = authorHash;
            //if (Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var requestHash) && authorHash == requestHash)
            //{
            //    return StatusCode(StatusCodes.Status304NotModified);
            //}
            //否则返回全部响应信息
            #endregion

            var name = System.Net.WebUtility.UrlEncode(resourcesParameters.AuthorName);
            var pageData = new
            {
                pageIndex = authors.PageIndex,
                pageSize = authors.PageSize,
                totalPages = authors.TotalPages,
                totalCount = authors.TotalCount,
                previousLink = authors.HasPrevious ? Url.Link(nameof(GetAuthorsAsync), new { pageindex = resourcesParameters.PageIndex - 1, pageSize = resourcesParameters.PageSize, authorName = name }) : null,
                nextLink = authors.HasNext ? Url.Link(nameof(GetAuthorsAsync), new { pageindex = resourcesParameters.PageIndex + 1, pageSize = resourcesParameters.PageSize, authorName = name }) : null,
                authorName = name
            };
            var str = JsonConvert.SerializeObject((pageData));
            Response.Headers.Add("PageMessage", JsonConvert.SerializeObject((pageData)));
            var authorDtos = mapper.Map<IEnumerable<AuthorDto>>(authors);
            var linkAuthorDtos = new ResponseColloctionResource<AuthorDto>(authorDtos.Select(author => CreateLinksForAuthor(author)).ToList());

            return CreateLinksForAuthors(linkAuthorDtos, pageData);

        }
        #endregion
        /// <summary>
        /// 根据Id获取用户信息
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpGet("{authorId}", Name = nameof(GetAuthorAsync))]
        public async Task<ActionResult<AuthorDto>> GetAuthorAsync(int authorId)
        {
            if (await repositoryWrapper.Author.IsExistAsync(authorId))
            {
                var author = await repositoryWrapper.Author.GetByIdAsync(authorId);
                return CreateLinksForAuthor(mapper.Map<AuthorDto>(author));
            }
            return NotFound();
        }

        /// <summary>
        /// 创建作者
        /// </summary>
        /// <param name="authorDto"></param>
        /// <returns></returns>
        [HttpPost(nameof(CreateAuthorAsync))]
        public async Task<IActionResult> CreateAuthorAsync(AuthorDto authorDto)
        {
            var author = mapper.Map<Author>(authorDto);
            repositoryWrapper.Author.Create(author);

            var result = await repositoryWrapper.Author.SaveAsync();
            if (!result)
            {
                throw new Exception("创建资源失败");
            }
            var authorCreate = mapper.Map<AuthorDto>(author);

            return CreatedAtRoute(nameof(GetAuthorAsync), new { authorId = authorCreate.Id }, authorCreate);
        }

        /// <summary>
        /// 删除作者
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpDelete("{authorId}", Name = nameof(DeleteAuthorAsync))]
        public async Task<IActionResult> DeleteAuthorAsync(int authorId)
        {
            var author = await repositoryWrapper.Author.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }
            repositoryWrapper.Author.Delete(author);
            if (!await repositoryWrapper.Author.SaveAsync())
            {
                throw  new  Exception("删除资源失败")
            }
           return NoContent();
        }

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="authourId"></param>
        /// <param name="authorDto"></param>
        /// <returns></returns>
        [HttpPut("{authourId}", Name = nameof(UpdateAuthorAsync))]
        public async Task<IActionResult> UpdateAuthorAsync(int authourId, UpdateAuthorDto authorDto)
        {
            var author = await repositoryWrapper.Author.GetByIdAsync(authourId);
            if (author == null)
            {
                return NotFound();
            }
            mapper.Map(authorDto, author);

            repositoryWrapper.Author.Update(author);
            var result = await repositoryWrapper.Author.SaveAsync();
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }

        /// <summary>
        /// 超文本驱动（实现HATEOAS 超媒体作为应用状态的引擎）
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        private AuthorDto CreateLinksForAuthor(AuthorDto author)
        {
            author.Links.Add(new Link(HttpMethods.Delete, "delete this author", Url.Link(nameof(DeleteAuthorAsync), new { authorId = author.Id })));
            author.Links.Add(new Link(HttpMethods.Put, "edit this author", Url.Link(nameof(UpdateAuthorAsync), new { authorId = author.Id })));
            author.Links.Add(new Link(HttpMethods.Get, "get author books", Url.Link(nameof(BooksController.GetBooksAsync), new { authorId = author.Id })));
            return author;
        }
         /// <summary>
        /// 超文本驱动（实现HATEOAS 超媒体作为应用状态的引擎）
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        private ResponseColloctionResource<AuthorDto> CreateLinksForAuthors(ResponseColloctionResource<AuthorDto> authors, dynamic pageData = null)
        {
            authors.Links.Add(new Link(HttpMethods.Post, "create a author", Url.Link(nameof(CreateAuthorAsync), null)));
            if (pageData != null)
            {
                authors.Links.Add(new Link(HttpMethods.Get, "previous", pageData.previousLink));

                authors.Links.Add(new Link(HttpMethods.Get, "next", pageData.nextLink));
            }
            return authors;
        }
    }
}
//await 告诉异步方法，必须等待await执行完，才往下执行
