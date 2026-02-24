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

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

			// Register PlacesService with explicit factory
			builder.Services.AddHttpClient<IPlacesService, PlacesService>().ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());
			builder.Services.AddTransient<IPlacesService>(serviceProvider =>
			{
				var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
				var httpClient = httpClientFactory.CreateClient();
				var configuration = serviceProvider.GetRequiredService<IConfiguration>();
				var apiKey = configuration["GooglePlaces:ApiKey"] ?? throw new InvalidOperationException("GooglePlaces:ApiKey is missing.");
                var repository = serviceProvider.GetRequiredService<IUserPlaceRepository>();
                return new PlacesService(httpClient, apiKey, repository);
			});
			builder.Services.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(
					builder.Configuration.GetConnectionString("DmytroNaumov"
                    )));

            builder.Services.AddScoped<IUserPlaceRepository, UserPlaceRepository>();
			builder.Services.AddScoped<ICustomPlacesService, CustomPlacesService>();

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			if (!app.Environment.IsDevelopment())
			{
				app.UseHttpsRedirection();
			}

			app.UseCors();

			//app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
