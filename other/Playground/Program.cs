using System;
using System.Threading.Tasks;
using AspNetCore.Identity.RavenDB;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {

            var userStore = new RavenDBUserStore(() => DocumentStoreHolder.Store.OpenAsyncSession());

            var task = userStore.CreateAsync(new RavenDBIdentityUser("Elemar JR"));
            Task.WaitAll(task);
            
            Console.WriteLine("Done!");
        }
    }
}