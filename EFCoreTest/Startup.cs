using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreTest.Entities;
using EFCoreTest.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper;
using EFCoreTest.Attributes;
using NLog.Web;
using NLog;
using NLog.Extensions.Logging;
using EFCoreTest.Filters;

namespace EFCoreTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //cors跨域配置
            services.AddCors(options => options.AddPolicy("first", builder => builder.AllowAnyOrigin()
            .AllowAnyMethod().AllowAnyHeader()));//.WithOrigins("http://localhost:59311")
            //添加异常过滤器config.Filters.Add<NlogExceptionFilter>(); 
            services.AddMvc(config => { config.Filters.Add<NlogExceptionFilter>(); }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);//允许返回接受xml.AddXmlSerializerFormatters()
            //配置数据库链接字符串
            services.AddDbContext<LibraryDbContext>(config =>
            {
                config.UseSqlServer(Configuration.GetSection("ConnectionString").GetValue<string>("ConStr"));
            });

            //注册仓储管理包
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            //注册AutoMapper映射工具包
            services.AddAutoMapper(typeof(Startup));
            //注册特性
            services.AddScoped<IsAuthorExistAttribute>();

            //注册响应缓冲中间件UseCaseSensitivePaths --是否区分大小写 MaximumBodySize消息体的缓冲大小设置
            services.AddResponseCaching();

            //注册memorycache
            services.AddMemoryCache();

            //注册identity服务
            //services.AddIdentity<User, Role>().AddEntityFrameworkStores<LibraryDbContext>();

            //注册api文档
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Library Api V1", Version = "v1" }));
            // c => c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Library API", Version = "v1" })

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)//, ILoggerFactory loggerFactory/ 过时的方法
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //loggerFactory.AddNLog();
            //env.ConfigureNLog("nlog.config");
            app.UseCors("first");
            app.UseHttpsRedirection();
            app.UseResponseCaching();
            app.UseSwagger();

            //配置要取的json地址v1要与上面注册的v1名字一样
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Api V1"));
            app.UseMvc();
        }
    }
}
