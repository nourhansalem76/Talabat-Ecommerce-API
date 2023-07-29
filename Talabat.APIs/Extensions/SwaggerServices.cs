namespace Talabat.APIs.Extensions
{
    public static class SwaggerServices
    {
        public static IServiceCollection AddSwaggerServices (this IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static WebApplication AddSwaggerMiddleware (this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();


            return app;
        }
    }
}
