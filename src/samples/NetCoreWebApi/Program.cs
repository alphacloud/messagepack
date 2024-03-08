namespace NetCoreWebApi;

using Microsoft.AspNetCore.Hosting;
using Serilog;


public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateWebHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, config) =>
            {
                config
                    .Enrich.FromLogContext()
                    .MinimumLevel.Verbose();
                config.WriteTo.Seq("http://localhost:5341");
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
