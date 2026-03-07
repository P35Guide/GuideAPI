using GuideAPI.Application.Interfaces;
using GuideAPI.Application.Services;
using GuideAPI.DAL;
using GuideAPI.DAL.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace GuideAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            // Google Places API key
            builder.Services.AddHttpClient<IPlacesService, PlacesService>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

            builder.Services.AddTransient<IPlacesService>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var apiKey = configuration["GooglePlaces:ApiKey"]
                    ?? throw new InvalidOperationException("GooglePlaces:ApiKey is missing.");
                var repository = serviceProvider.GetRequiredService<IUserPlaceRepository>();
                return new PlacesService(httpClient, apiKey, repository);
            });

            // PostgreSQL — спочатку перевіряє змінну середовища DATABASE_URL (Render/Supabase),
            // потім ConnectionStrings:KulbachukViacheslav з appsettings.json (локально)
            var rawConnectionString =
                Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? builder.Configuration.GetConnectionString("KulbachukViacheslav")
                ?? throw new InvalidOperationException("Database connection string not found.");

            // Конвертуємо postgresql:// URI у формат який розуміє Npgsql
            // (Supabase дає URI, Npgsql очікує Host=...;Database=...;)
            var connectionString = ConvertDatabaseUrl(rawConnectionString);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsAssembly("GuideAPI.DAL")
                ));

            builder.Services.AddScoped<IUserPlaceRepository, UserPlaceRepository>();
            builder.Services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
            builder.Services.AddScoped<ITelegramUserService, TelegramUserService>();
            builder.Services.AddScoped<ICustomPlacesService, CustomPlacesService>();

            var app = builder.Build();

            // Автоматично застосовуємо міграції при старті (зручно для Render)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            // Swagger доступний завжди (можна прибрати в prod якщо не потрібно)
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
        // Конвертує postgresql://user:password@host:port/database → Host=...;Username=...;...
        private static string ConvertDatabaseUrl(string url)
        {
            if (!url.StartsWith("postgresql://") && !url.StartsWith("postgres://"))
                return url; // вже у правильному форматі (локальний appsettings)

            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.TrimStart('/');

            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
        }
    }
}
