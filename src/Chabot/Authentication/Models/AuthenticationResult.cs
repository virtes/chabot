using System.Collections.Generic;
using System.Security.Claims;

// ReSharper disable once CheckNamespace
namespace Chabot.Authentication
{
    public readonly struct AuthenticationResult
    {
        private AuthenticationResult(bool succeeded, string? stateKey, List<Claim> claims)
        {
            Succeeded = succeeded;
            StateKey = stateKey;
            Claims = claims;
        }

        public bool Succeeded { get; }

        public string? StateKey { get; }

        public List<Claim> Claims { get; }

        public static AuthenticationResult Success(string stateKey, List<Claim> claims)
        {
            return new AuthenticationResult(true, stateKey, claims);
        }

        public static AuthenticationResult Failed()
        {
            return new AuthenticationResult(false, null, null!);
        }
    }
}