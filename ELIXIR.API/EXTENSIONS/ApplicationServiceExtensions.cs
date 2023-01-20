using ELIXIR.DATA.CORE.ICONFIGURATION;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using Microsoft.Extensions.DependencyInjection;

namespace ELIXIR.API.EXTENSIONS
{
    public static class ApplicationServiceExtensions
    {

        public static IServiceCollection AddApplicationServices(
                               this IServiceCollection services)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

    }
}
