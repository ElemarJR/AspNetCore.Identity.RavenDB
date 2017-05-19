using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Identity.RavenDB;
using Microsoft.AspNetCore.Identity;
using Raven.Client;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {

            var userStore = new RavenDBUserStore(DocumentStoreHolder.Store);

            var user = userStore.FindByIdAsync("RavenDBIdentityUsers/1").Result;
            userStore.SetPhoneNumberAsync(user, "+5554999420609").Wait();
            userStore.SetPhoneNumberConfirmedAsync(user, true);
            userStore.UpdateAsync(user).Wait();
            //userStore.SetEmailAsync(user, "me@elemarjr.com").Wait();
            //userStore.SetEmailConfirmedAsync(user, true);
            //userStore.UpdateAsync(user).Wait();

            //var userRecovered = userStore.FindByEmailAsync("me@elemarjr.com").Result;
            // REPLACING A CLAIM
            //var user = userStore.FindByIdAsync("RavenDBIdentityUsers/1").Result;
            //userStore.ReplaceClaimAsync(
            //    user,
            //    new Claim(ClaimTypes.Country, "Brazil"),
            //    new Claim(ClaimTypes.Country, "Israel")
            //).Wait();

            //userStore.UpdateAsync(user).Wait();


            // GETTING USERS USING CLAIMS
            //var users = userStore.GetUsersForClaimAsync(
            //    new Claim(ClaimTypes.Country, "Brazil")
            //).Result;
            //foreach (var user in users)
            //{
            //    Console.WriteLine(user.UserName);
            //}

            // ADDING CLAIMS TO USERS
            //var user = userStore.FindByIdAsync("RavenDBIdentityUsers/1").Result;
            //userStore.AddClaimsAsync(user, new[]
            //{
            //    new Claim(ClaimTypes.Country, "Brazil")
            //}).Wait();
            //userStore.UpdateAsync(user).Wait();

            //RavenDBIdentityUser newUser;
            //userStore.CreateAsync(newUser = new RavenDBIdentityUser("Juliana de Paula")).Wait();
            //userStore.AddClaimsAsync(newUser, new[]
            //{
            //    new Claim(ClaimTypes.Country, "Brazil")
            //}).Wait();
            //userStore.UpdateAsync(newUser).Wait();

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