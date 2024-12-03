using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuantumBasedQuantTrading.Logic.Interface;
using QuantumBasedQuantTrading.Logic.Logics;
using QuantumBasedQuantTrading.Models;
using QuantumBasedQuantTrading.Repository.Data;
using QuantumBasedQuantTrading.Repository.Interface;
using QuantumBasedQuantTrading.Repository.Repos;
using System.Configuration;

namespace QuantumBasedQuantTrading.APIEndpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllersWithViews();
            var connectionString = builder.Configuration.GetConnectionString("AzureConnection");
            builder.Services.AddDbContext<QuantumBasedQuantTradingDbContext>(option =>
            {
                option
                .UseSqlServer(connectionString)
                .UseLazyLoadingProxies();
            });

            builder.Services.AddDefaultIdentity<Users>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 2;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<QuantumBasedQuantTradingDbContext>();

            builder.Services.AddTransient<IRepository<AllArticles>, AllArticlesRepository>();
            builder.Services.AddTransient<IRepository<FulloutAllArticles>, FullOutRepository>();
            builder.Services.AddTransient<IRepository<OutputSent>, OutputSentRepository>();
            builder.Services.AddTransient<IRepository<AverageSent>, AverageSentRepository>();
            builder.Services.AddTransient<IRepository<RequestParameters>, RequestParametersRepository>();
            builder.Services.AddTransient<IRepository<MachineLearningModelData>, MachineLearningModelDataRepository>();

            builder.Services.AddTransient<IAllArticlesLogic, AllArticlesLogic>();
            builder.Services.AddTransient<IFullOutLogic, FullOutLogic>();
            builder.Services.AddTransient<IOutputSentLogic, OutputSentLogic>();
            builder.Services.AddTransient<IAverageSentLogic, AverageSentLogic>();
            builder.Services.AddTransient<IBusinessLogic, BusinessLogic>();
            builder.Services.Configure<Settings>(builder.Configuration.GetSection("ApiKeys"));
            builder.Services.AddSingleton<ISettings>(sp =>
                sp.GetRequiredService<IOptions<Settings>>().Value);


            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(t =>
            {
                t.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "QuantumBasedQuantTrading.Endpoint",
                    Version = "v1",
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(t => t.SwaggerEndpoint("/swagger/v1/swagger.json", "QuantumBasedQuantTrading.Endpoint v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();

        }
    }
}