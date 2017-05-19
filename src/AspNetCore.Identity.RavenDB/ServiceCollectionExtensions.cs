using Microsoft.Extensions.DependencyInjection;
using Raven.Client;

namespace AspNetCore.Identity.RavenDB
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRavenDocumentStore<TDocumentStore>(
            this IServiceCollection services,
            TDocumentStore documentStore
        ) where TDocumentStore : class, IDocumentStore
        {
            return services.AddSingleton(documentStore);
        }
    }
}
