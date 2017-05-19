using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raven.Client;

namespace AspNetCore.Identity.RavenDB
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder UseRavenDBDataStoreAdaptor<TDocumentStore>(
            this IdentityBuilder builder
        ) where TDocumentStore : class, IDocumentStore
            => builder
                .AddRavenDBUserStore<TDocumentStore>()
                .AddRavenDBRoleStore<TDocumentStore>();
        
        private static IdentityBuilder AddRavenDBUserStore<TDocumentStore>(
            this IdentityBuilder builder
        )
        {
            var userStoreType = typeof(RavenDBUserStore<,>).MakeGenericType(builder.UserType, typeof(TDocumentStore));

            builder.Services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(builder.UserType),
                userStoreType
            );

            return builder;
        }

        private static IdentityBuilder AddRavenDBRoleStore<TDocumentStore>(
            this IdentityBuilder builder
        )
        {
            var roleStoreType = typeof(RavenDBRoleStore<,>).MakeGenericType(builder.RoleType, typeof(TDocumentStore));

            builder.Services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
                roleStoreType
            );

            return builder;
        }
    }
}
