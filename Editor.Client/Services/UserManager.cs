using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Editor.Client.Services
{
    public class UserManager
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public UserManager(AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<ClaimsPrincipal?> GetUserAsync()
        {
            var authState = await authenticationStateProvider
            .GetAuthenticationStateAsync();
            return authState.User;
        }
    }
}
