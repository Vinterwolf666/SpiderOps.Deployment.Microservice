
using Deployment.Microservice.APP;
using Deployment.Microservice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Infrastructure.Microservice.Domain;
using Infrastructure.Microservice.Infrastructure;
using Infrastructure.Microservice.APP;
using Deployment.Microservice.Service;
using GCPInfrastructureServices = Deployment.Microservice.APP.GCPInfrastructureServices;

namespace Deployment.Microservice.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var configuration = builder.Configuration;

         
            builder.Services.AddDbContext<DeploymentsDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Value"), b => b.MigrationsAssembly("Deployment.Microservice.DEP.API")));
            builder.Services.AddDbContext<GCPInfrastructureDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Value2"), b => b.MigrationsAssembly("Deployment.Microservice.DEP.API")));
            builder.Services.AddDbContext<GCPTemplatesDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Value2"), b => b.MigrationsAssembly("Deployment.Microservice.DEP.API")));
            builder.Services.AddDbContext<CustomPipelinesDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Value3"), b => b.MigrationsAssembly("Deployment.Microservice.DEP.API")));
            builder.Services.AddDbContext<PipelinesDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("Value"), b => b.MigrationsAssembly("Deployment.Microservice.DEP.API")));



            builder.Services.AddScoped<IPipelinesRepository, PipelinesRepository>();
            builder.Services.AddScoped<IPipelinesServices, PipelinesServices>();


            builder.Services.AddScoped<IDeploymentsRepository, DeploymentsRepository>();
            builder.Services.AddScoped<IDeploymentsServices, DeploymentsServices>();

            builder.Services.AddScoped<IGCPInfrastructureRepository, GCPInfrastructureRepository>();
            builder.Services.AddScoped<IGCPInfrastructureServices, GCPInfrastructureServices>();

            builder.Services.AddScoped<IGCPTemplatesRepository, GCPTemplatesRepository>();
            builder.Services.AddScoped<IGCPTemplatesServices, GCPTemplatesServices>();

            builder.Services.AddScoped<ICustomerPipelinesRepository, CustomPipelinesRepository>();
            builder.Services.AddScoped<ICustomPipelinesServices, CustomPipelinesServices>();

           



            builder.Services.AddCors(options =>
            {

                options.AddPolicy("nuevaPolitica", app =>
                {

                    app.AllowAnyOrigin();
                    app.AllowAnyHeader();
                    app.AllowAnyMethod();
                });

            });

            var app = builder.Build();

           
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("nuevaPolitica");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}