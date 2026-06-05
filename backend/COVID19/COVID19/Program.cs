using COVID19.Data;
using COVID19.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace COVID19
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            builder.Services.AddControllers().AddOData(options =>
                options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(10000)
                    .AddRouteComponents("odata", GetEdmModel()));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<Covid19DbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<CountryRegion>("CountryRegions");
            builder.EntitySet<ProvinceState>("ProvinceStates");
            builder.EntitySet<CovidGlobalConfirmed>("CovidGlobalConfirmeds");
            builder.EntitySet<CovidGlobalDeaths>("CovidGlobalDeaths");
            builder.EntitySet<CovidGlobalRecovered>("CovidGlobalRecovereds");
            builder.EntitySet<DailyReport>("DailyReport");
            return builder.GetEdmModel();
        }
    }
}
