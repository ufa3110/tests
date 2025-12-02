using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using TZTaskManager.Application.Interfaces;
using TZTaskManager.Application.Mappings;
using TZTaskManager.Application.Services;
using TZTaskManager.Application.Validators;
using TZTaskManager.Domain.Interfaces;
using TZTaskManager.Infrastructure.Data;
using TZTaskManager.Presentation.Middleware;

namespace TZTaskManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidator>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "TZ Task Manager API",
                    Version = "v1",
                    Description = "API для управления задачами"
                });
            });

            var app = builder.Build();

            var applyMigrationsOnStartup = builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup", true);
            if (applyMigrationsOnStartup)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<ApplicationDbContext>();
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Произошла ошибка при применении миграций базы данных");
                        if (!app.Environment.IsDevelopment())
                        {
                            throw;
                        }
                    }
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
