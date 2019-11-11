using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SSMVCCoreApp.Models;
using SSMVCCoreApp.Models.Abstract;
using SSMVCCoreApp.Models.Concrete;
using SSMVCCoreApp.Models.Services;

namespace SSMVCCoreApp
{
  public class Startup
  {
    private ILogger<Startup> _logger;

    public Startup(IConfiguration configuration, ILogger<Startup> logger)
    {
      Configuration = configuration;
      _logger = logger;
    }
    public IConfiguration Configuration { get; private set; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<StorageUtility>(cfg =>
      {
        if (string.IsNullOrEmpty(Configuration["StorageAccountInformation"]))
        {
          cfg.StorageAccountName = Configuration["StorageAccountInformation:StorageAccountName"];
          cfg.StorageAccountAccessKey = Configuration["StorageAccountInformation:StorageAccountAccessKey"];
        }
      });

      services.AddMvc();

      services.AddDbContext<SportsStoreDbContext>(cfg =>
      {
        //var connectionString = Configuration.GetConnectionString("SportsStoreConnection");

        cfg.UseSqlServer(Configuration["ConnectionStrings:SportsStoreConnection"], sqlServerOptionsAction: sqlOption =>
        {
          //This is for the Resilient Entity Framework Core SQL connections (Similar to SqlAzureExecutionStrategy in MVC5)
          sqlOption.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        });
      });

      services.AddScoped<IProductRepository, EfProductRepository>();
      services.AddScoped<IPhotoService, PhotoService>();

      services.AddApplicationInsightsTelemetry(cfg =>
      {
        cfg.InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];
      });

      services.AddLogging(cfg =>
      {
        cfg.AddApplicationInsights(Configuration["ApplicationInsights:InstrumentationKey"]);
        // Optional: Apply filters to configure LogLevel Information or above is sent to
        // ApplicationInsights for all categories.
        cfg.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);

        // Additional filtering For category starting in "Microsoft",
        // only Warning or above will be sent to Application Insights.
        //cfg.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
      });
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      var appInsightsFlag = app.ApplicationServices.GetService<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>();
      if (Configuration["EnableAppInsightsDisableTelemetry"] == "false")
      {
        appInsightsFlag.DisableTelemetry = false;
      }
      else
      {
        appInsightsFlag.DisableTelemetry = true;
      }

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseFileServer(); /* this will be used to load css, js file */

      app.UseMvc(ConfigureRoutes);

      using (var scope = app.ApplicationServices.CreateScope())
      {
        var context = scope.ServiceProvider.GetRequiredService<SportsStoreDbContext>();
        // This will ensure the database is created and the migrations are applied
        context.Database.Migrate();
        // If database does not exist then the database and all its schema are created and also it ensures it is compatible with the model for this context.
        //context.Database.EnsureCreated();
      }

      app.Run(async (context) =>
      {
        await context.Response.WriteAsync($"<div style='background-color:cornflowerblue; text-align:Center; color:White;'><h1>Sports Store Site is Under Construction</h1><h4>{Configuration["MOD"]} - {DateTime.Now}</h4></div>");
      });
    }

    private void ConfigureRoutes(IRouteBuilder routeBuilder)
    {
      routeBuilder.MapRoute("Default", "{controller=Product}/{action=Index}/{id?}");
      //routeBuilder.MapRoute("Default", "{controller}/{action}/{id?}");
    }
  }
}
