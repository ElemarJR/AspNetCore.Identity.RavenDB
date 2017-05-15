using System;
using System.Threading.Tasks;
using AspNetCore.Identity.RavenDB;
using Microsoft.AspNetCore.Identity;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {

            var userStore = new RavenDBUserStore(() => DocumentStoreHolder.Store.OpenAsyncSession());

            // ADD LOGIN INFO
            //var user = userStore.FindByIdAsync("RavenDBIdentityUsers/1").Result;
            //userStore.AddLoginAsync(
            //    user, 
            //    new UserLoginInfo("abc", "xpto", "My Social Network")
            //    );
            //Task.WaitAll(userStore.UpdateAsync(user));

            // FIND BY LOGIN INFO
            //var user = userStore.FindByLoginAsync("abc", "xpto").Result;
            //Console.WriteLine(user.UserName);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}