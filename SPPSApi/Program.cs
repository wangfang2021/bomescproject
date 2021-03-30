using System;
using System.Collections.Generic;
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
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:5000;");//ע�����ӿ����ã���������������

                    //webBuilder.UseStartup<Startup>().UseUrls("http://*:5000;https://*:5001;")
                    //.UseKestrel(option =>
                    // {
                    //     option.ConfigureHttpsDefaults(i =>
                    //     {
                    //         i.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("C:\\tmp\\SPPS.pfx", "huj123432");
                    //     });
                    // })
                    //;//ע�����ӿ����ã���������������

                });
    }
}
