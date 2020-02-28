using EFCoreTest.Helpes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreTest.Filters
{
    public class NlogExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<Program> logger;

        public NlogExceptionFilter(IHostingEnvironment env, ILogger<Program> logger)
        {
            this.env = env;
            this.logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            var error = new ApiError();
            //针对不同运行环境的处理
            if (env.IsDevelopment())
            {
                error.Message = context.Exception.Message;
                error.Detail = context.Exception.ToString();
            }
            else
            {
                error.Message = "服务器错误";
                error.Detail = context.Exception.Message;
            }
            //更改内容为errpr
            context.Result = new ObjectResult(error) { StatusCode = StatusCodes.Status500InternalServerError };
            StringBuilder sb = new StringBuilder();
            sb.Append($"服务器异常：{context.Exception.Message}");
            sb.Append($" {context.Exception.ToString()}");
            logger.LogCritical(sb.ToString());
        }
    }
}
