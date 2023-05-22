using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace CMS
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment HostEnvironment;

        public Startup(IConfiguration configuration,
            IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region BaseServices

            var mvcBuilder = services.AddControllersWithViews();
            if (HostEnvironment.IsDevelopment())
                mvcBuilder.AddRazorRuntimeCompilation();

            mvcBuilder.AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = null,
                };
            });

            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddSession(s => { s.IdleTimeout = TimeSpan.FromHours(2); });
            services.AddScoped<IBaseModel, BaseModel>();
            services.AddScoped<IBaseSession, BaseSession>();
            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISendMail, SendMail>();
            services.AddHttpContextAccessor();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddKendo();
            #endregion



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            if (HostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            SessionRequest.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());


            app.UseSession();
            app.UseCookiePolicy();
            app.UseMiddleware<ErrorMid>();


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseMvcWithDefaultRoute();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(name: "default", template: "{controller=" + "Base" + "}/{action=" + "Login" + "}/{Id?}");
            //});

            //app.UseMvc(routes => { routes.MapRoute(name: "default", template: "{controller=Base}/{action=Login}/{id?}"); });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Base}/{action=Login}/{id?}");
            });


        }
    }
}
