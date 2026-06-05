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
                    policy.WithOrigins(
                            "http://localhost:5173",
                            "https://localhost:5173",
                            "http://localhost:5174",
                            "https://localhost:5174",
                            "http://localhost:5175",
                            "https://localhost:5175",
                            "http://127.0.0.1:5173",
                            "https://127.0.0.1:5173",
                            "http://127.0.0.1:5174",
                            "https://127.0.0.1:5174",
                            "http://127.0.0.1:5175",
                            "https://127.0.0.1:5175")
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
