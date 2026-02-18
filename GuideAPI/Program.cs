
using GuideAPI.Application.Interfaces;
using GuideAPI.Application.Services;

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

            //Register Services
            builder.Services.AddHttpClient<IPlacesService, PlacesService>()
                .AddTypedClient((httpClient, serviceProvider) =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var apiKey = configuration["GooglePlaces:ApiKey"];
        return new PlacesService(httpClient, apiKey);
    });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
