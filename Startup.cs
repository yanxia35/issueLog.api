using System.Net;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using IssueLog.API.Data;
using IssueLog.API.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace IssueLog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
            .AddNewtonsoftJson(opt=>{
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddDbContext<DataContext>(x => x.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<TempContext>(x => x.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<PacsContext>(x => x.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
             AddJwtBearer(options =>
            {
                // string json = JsonConvert.SerializeObject(options,Formatting.Indented);
                // System.Console.WriteLine(json);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    SaveSigninToken = true
                };
            });

            services.AddCors();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IIssueRepository, IssueRepository>();
            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<IFailureModeRepository, FailureModeRepository>();
            services.AddScoped<IActionRepository, ActionRepository>();
            services.AddScoped<IPartIssueRepository, PartIssueRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISubscriberRepository, SubscriberRepository>();
            services.AddScoped<IEcPartRepository, EcPartRepository>();
            services.AddScoped<IIssueFileRepository, IssueFileRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IKpiRepo, KpiRepo>();
            services.AddScoped<IMbomRepo, MbomRepo>();
            services.AddHostedService<ScheduleTaskService>(); // add a parallel service which query late action in database and send email to acition owner 
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = (int) HttpStatusCode.TemporaryRedirect;
                options.HttpsPort = 443;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            if (error.Error.InnerException != null)
                            {
                                context.Response.AddApplicationError(error.Error.InnerException.Message);
                                await context.Response.WriteAsync(error.Error.InnerException.Message);
                            }
                            else
                            {
                                context.Response.AddApplicationError(error.Error.Message);
                                await context.Response.WriteAsync(error.Error.Message);
                            }

                        }
                    });
                });
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>{
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index","Fallback");
            });
        //     app.UseMvc(routes =>
        //                     {
        //                         routes.MapSpaFallbackRoute(
        //                             name: "spa-fallback",
        //                             defaults: new { controller = "Fallback", action = "Index" }
        //                         );
        //                     }
        //                 );

        }
    }
}