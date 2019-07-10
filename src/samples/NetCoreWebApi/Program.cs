namespace NetCoreWebApi
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Serilog;


    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, config) =>
                {
                    config
                        .Enrich.FromLogContext()
                        .MinimumLevel.Verbose();
                    config.WriteTo.Seq("http://localhost:5341");
                })
                .UseStartup<Startup>();
        }
    }
}
