using System.Text.Json.Serialization;
using Domain;
using Task6Itransition.Services;

namespace Task6Itransition_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddSignalR()
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
