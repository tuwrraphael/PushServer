using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PushServer.Configuration;
using PushServer.PushConfiguration.EntityFramework.Extensions;
using PushServer.AzureNotificationHub;

namespace DigitPushService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
            environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = $"Data Source={HostingEnvironment.WebRootPath}\\App_Data\\pushservice.db";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddPushServer()
                .AddConfigurationStore(builder =>
                            builder.UseSqlite(connectionString,
                                sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddAzureNotificationHub(v => new AzureNotificationHubConfig() { });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration["ServiceIdentityUrl"];
                    options.Audience = "digit";
                    options.RequireHttpsMetadata = false;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireClaim("scope", "digit.user");
                });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Service", builder =>
                {
                    builder.RequireClaim("scope", "digit.service");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}
