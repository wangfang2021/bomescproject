using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SPPSApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseStartup<Startup>().UseUrls("http://*:8097;");//注意这块接口配置，才能让外网访问
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:5000;");

                    //webBuilder.UseStartup<Startup>().UseUrls("http://*:5000;https://*:5001;")
                    //.UseKestrel(option =>
                    // {
                    //     option.ConfigureHttpsDefaults(i =>
                    //     {
                    //         string strDir = AppDomain.CurrentDomain.BaseDirectory;
                    //         i.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(strDir+"SPPS.pfx", "huj123432");
                    //     });
                    // })
                    //;//注意这块接口配置，才能让外网访问

                });
    }
}
