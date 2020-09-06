using ConcurrentAccessRestriction.Interface;
using ConcurrentAccessRestriction.Storage;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConcurrentAccessRestriction.WebClient
{
    internal class AzureADOpenIdConnectOptionsConfiguration : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly IOptionsMonitor<AzureADOptions> _azureADOptions;

        public AzureADOpenIdConnectOptionsConfiguration(IOptionsMonitor<AzureADOptions> azureADOptions)
        {
            _azureADOptions = azureADOptions;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            var azureADOptions = _azureADOptions.Get(AzureADDefaults.AuthenticationScheme);
            if (name != azureADOptions.OpenIdConnectSchemeName)
            {
                return;
            }

            options.ClientId = azureADOptions.ClientId;
            options.ClientSecret = azureADOptions.ClientSecret;
            options.Authority = new Uri(new Uri(azureADOptions.Instance), azureADOptions.TenantId).ToString();
            options.CallbackPath = azureADOptions.CallbackPath ?? options.CallbackPath;
            options.SignedOutCallbackPath = azureADOptions.SignedOutCallbackPath ?? options.SignedOutCallbackPath;
            options.SignInScheme = azureADOptions.CookieSchemeName;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.UseTokenLifetime = true;
            options.Events = new OpenIdConnectEvents()
            {
                OnTicketReceived = context =>
                {
                    var sessionService = context.HttpContext.RequestServices.GetRequiredService<ISessionService>();
                    var claimIdentity = new ClaimsIdentity("AppUser");
                    var sessionId = Guid.NewGuid().ToString();
                    claimIdentity.AddClaim(new Claim("sessionId", sessionId));
                    sessionService.AddSession(sessionId, context.Principal.Identity.Name);
                    context.Principal.AddIdentity(claimIdentity);
                    return Task.CompletedTask;
                }
            };
        }

       
        public void Configure(OpenIdConnectOptions options)
        {
        }
    }
}