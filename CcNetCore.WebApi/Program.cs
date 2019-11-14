using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CcNetCore.WebApi {
    public class Program {
        public static void Main (string[] args) {
            CreateWebHostBuilder (args).Build ().Run ();
        }

        public static IWebHostBuilder CreateWebHostBuilder (string[] args) {
            var configuration = new ConfigurationBuilder ()
                .SetBasePath (Environment.CurrentDirectory)
                .AddJsonFile ("host.json")
                .Build ();

            var urls = configuration["urls"];

            return WebHost.CreateDefaultBuilder (args)
                .UseUrls (urls)
                .UseStartup<Startup> ();
        }
    }
}