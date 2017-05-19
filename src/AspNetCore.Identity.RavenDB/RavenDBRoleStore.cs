using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Raven.Abstractions.Exceptions;
using Raven.Client;

namespace AspNetCore.Identity.RavenDB
{
    public class RavenDBRoleStore<TRole, TDocumentStore> :
        IQueryableRoleStore<TRole>,
        IRoleClaimStore<TRole>
        where TRole : RavenDBIdentityRole
        where TDocumentStore : class, IDocumentStore
    {
        public IdentityErrorDescriber ErrorDescriber { get; }
        public TDocumentStore Context { get; }

        private readonly Lazy<IAsyncDocumentSession> _session;

        public RavenDBRoleStore(
            TDocumentStore context,
            IdentityErrorDescriber errorDescriber = null
        )
        {
            ErrorDescriber = errorDescriber;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            _session = new Lazy<IAsyncDocumentSession>(() =>
            {
                var session = Context.OpenAsyncSession();
                session.Advanced.UseOptimisticConcurrency = true;
                return session;
            }, true);
        }

        public IAsyncDocumentSession Session
            => _session.Value;

        public IQueryable<TRole> Roles => throw new NotSupportedException();

        public Task SaveChanges(
            CancellationToken cancellationToken = default(CancellationToken)
        ) => Session.SaveChangesAsync(cancellationToken);


        #region IDisposable
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private bool _disposed;
        public void Dispose()
        {
            _disposed = true;
        }
        #endregion

        #region IQueryableRoleStore
        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await Session.StoreAsync(role, cancellationToken);
            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var stored = await Session.LoadAsync<TRole>(role.Id, cancellationToken);
            var etag = Session.Advanced.GetEtagFor(stored);

            await Session.StoreAsync(role, etag, role.Id, cancellationToken);

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (ConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            Session.Delete(role.Id);

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (ConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Session.LoadAsync<TRole>(roleId, cancellationToken);
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Session.Query<TRole>().FirstOrDefaultAsync(
                role => role.NormalizedName == normalizedRoleName, token: cancellationToken
                );
        }
        #endregion

        #region IRoleClaimStore
        public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult<IList<Claim>>(role.Claims.Select(c => new Claim(c.Type, c.Value)).ToList());

        }

        public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            role.AddClaim(claim);

            return Task.CompletedTask;
        }

        public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            role.RemoveClaim(claim);
            
            return Task.CompletedTask;
        }
        #endregion
    }
}
