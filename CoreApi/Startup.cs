using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Entities;
using CoreApi.Repositories;
using CoreApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;

namespace CoreApi
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();// 注册MVC到Container
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else

            services.AddTransient<IMailService, CloudMailService>();
#endif

            //services.AddDbContext<Entities.MyContext>();
            var connectionString = Configuration["connectionStrings:productionInfoDbConnectionString"];
            services.AddDbContext<Entities.MyContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<IProductRepository, ProductRepository>();

            ////core默认使用json.net对结果默认做了camel case的转化(大概可理解为首字母小写)，这句话会去除默认转换
            //services.AddMvc().AddJsonOptions(options =>
            //    {
            //        if (options.SerializerSettings.ContractResolver is Newtonsoft.Json.Serialization.DefaultContractResolver resolver)
            //        {
            //            resolver.NamingStrategy = null;
            //        }
            //    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory, MyContext myContext)
        {
            //针对LoggerFactory.AddProvider()这种写法，Nlog一个简单的ExtensionMethod做了这个工作，就是AddNlog();
            // loggerFactory.AddProvider(new NLogLoggerProvider());s
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //处理异常
                app.UseExceptionHandler();
            }

            myContext.EnsureSeedDataForContext();

            //对404等状态返回友好提示
            app.UseStatusCodePages();

            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
