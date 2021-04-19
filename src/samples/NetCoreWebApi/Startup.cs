namespace NetCoreWebApi
{
    using Alphacloud.MessagePack.AspNetCore.Formatters;
    using MessagePack;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
#if NETCOREAPP3_1 || NET5_0 || NET6_0
    using Microsoft.Extensions.Hosting;
#endif
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMessagePack(options =>
            {
                options.Compression = MessagePackCompression.Lz4Block;

            });
#if NETCOREAPP3_1 || NET5_0 || NET6_0
            services.AddControllers()
                .AddNewtonsoftJson()
#else
            services.AddMvc()
#if NETCOREAPP2_1
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
#endif
#endif
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
#if NETCOREAPP3_1|| NET5_0 || NET6_0
            IWebHostEnvironment env
#else
            IHostingEnvironment env
#endif
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
#if NETCOREAPP3_1 || NET5_0 || NET6_0
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

#else
            app.UseMvc();
#endif

        }
    }
}
