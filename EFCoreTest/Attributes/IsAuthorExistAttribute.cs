using EFCoreTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreTest.Attributes
{
    /// <summary>
    ///  作者过滤器（用于bookControll）
    /// </summary>
    public class IsAuthorExistAttribute : ActionFilterAttribute
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public IsAuthorExistAttribute(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var id = (int)context.ActionArguments.Single(m => m.Key == "authorId").Value;
            var result = await repositoryWrapper.Author.IsExistAsync(id);
            if (!result)
            {
                context.Result = new NotFoundResult();
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
