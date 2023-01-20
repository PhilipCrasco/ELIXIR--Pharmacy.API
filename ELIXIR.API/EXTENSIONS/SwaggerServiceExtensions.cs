using ELIXIR.API.MIDDLEWARE;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ELIXIR.API.EXTENSIONS
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(
                                this IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            return services;
        }

        public static IApplicationBuilder UserSwaggerDocumentation(
                                      this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleWare>();

            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "My API V1");
            });
            //app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ELIXIR.API v1"));

            app.UseHttpsRedirection();

       
            app.UseAuthentication();



            return app;
        }
    }
}
