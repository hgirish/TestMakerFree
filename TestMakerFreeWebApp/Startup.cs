using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;

namespace TestMakerFreeWebApp {
  public class Startup {
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc()
  .AddJsonOptions(opt => {
    opt.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
  });




      services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(
          Configuration.GetConnectionString("DefaultConnection")));

      services.AddIdentity<ApplicationUser, IdentityRole>(opts => {
        opts.Password.RequireDigit = true;
        opts.Password.RequireLowercase = true;
        opts.Password.RequireUppercase = true;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequiredLength = 7;
      }).AddEntityFrameworkStores<ApplicationDbContext>();

      services.AddEntityFrameworkSqlServer();

      services.AddAuthentication(opts => {
        opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(cfg => {
        cfg.RequireHttpsMetadata = false;
        cfg.SaveToken = true;
        cfg.TokenValidationParameters = new TokenValidationParameters {
          ValidIssuer = Configuration["Auth:Jwt:Issuer"],
          ValidAudience = Configuration["Auth:Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"])),
          ClockSkew = TimeSpan.Zero,
          RequireExpirationTime = true,
          ValidateIssuer = true,
          ValidateIssuerSigningKey = true,
          ValidateAudience = true
        };
      });



      // In production, the Angular files will be served from this directory
      services.AddSpaStaticFiles(configuration => {
        configuration.RootPath = "ClientApp/dist";
      });

     // services.AddSingleton<IConfiguration>(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }
      else {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseAuthentication();

      app.UseMvc(routes => {
        routes.MapRoute(
            name: "default",
            template: "{controller}/{action=Index}/{id?}");
      });

      app.UseSpa(spa => {
        // To learn more about options for serving an Angular SPA from ASP.NET Core,
        // see https://go.microsoft.com/fwlink/?linkid=864501

        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment()) {
              //spa.UseAngularCliServer(npmScript: "start");
              spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
          }
      });
    }
  }
}
