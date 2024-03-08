namespace NetCoreWebApi;

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
            .UseSerilog((_, _, config) =>
            {
                config
                    .Enrich.FromLogContext()
                    .MinimumLevel.Verbose();
                config.WriteTo.Seq("http://localhost:5341");
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
