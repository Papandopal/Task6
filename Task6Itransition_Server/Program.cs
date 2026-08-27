using System.Text.Json.Serialization;
using Domain;
using Microsoft.EntityFrameworkCore;
using Task6Itransition.Services;
using Task6Itransition_Server.Services.Database;

namespace Task6Itransition_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddSignalR(options =>
            {
                options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
            })
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                    policy.WithOrigins(builder.Configuration["WASMUrl"]!)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
            });

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.MapHub<CenterHub>("/hub");

            app.MapControllers();

            app.Run();
        }
    }
}
