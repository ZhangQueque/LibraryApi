using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace EFCoreTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().ConfigureLogging(logging =>
               {
                   //logging.AddNLog();
                   //logging.AddConsole();
                   //NLog.Web.NLogBuilder.ConfigureNLog("nlog.config");
                   logging.ClearProviders();
                   logging.SetMinimumLevel(LogLevel.Information);
                   logging.AddConsole();
               }).UseNLog();
    }
}
