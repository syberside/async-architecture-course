using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace aTES.Identity.Configs
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource(
                    name: "PopugRole",
                    displayName: "PopugRole",
                    userClaims: new[] { "PopugRole", }
                    ),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // machine to machine client (oAuth)
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api1" }
                },

                // interactive ASP.NET Core MVC client (cookie based)
                new Client
                {
                    ClientId = "billing",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5021/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5021/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                         "PopugRole"
                    },
                    Claims=new []
                    {
                        new ClientClaim("PopugRole", "PopugRole")
                    }
                },
                // interactive ASP.NET Core MVC client (cookie based)
                new Client
                {
                    ClientId = "task-tracker",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5011/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5011/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                         "PopugRole"
                    },
                    Claims=new []
                    {
                        new ClientClaim("PopugRole", "PopugRole")
                    }
                }
            };
    }
}
