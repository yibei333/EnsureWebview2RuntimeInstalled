using Microsoft.Extensions.DependencyInjection;
using SharpDevLib.Extensions.Http;

namespace EnsureWebview2RuntimeInstalled;

public static class ServiceUtil
{
    private static IServiceProvider? serviceProvider;

    public static IServiceProvider ServiceProvider
    {
        get
        {
            if (serviceProvider is not null) return serviceProvider;
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IHttpService, IHttpService>();
            serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }

    public static IHttpService HttpService => ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IHttpService>();
}
