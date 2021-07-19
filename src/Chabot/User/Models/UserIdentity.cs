using System.Collections.Generic;
using System.Security.Claims;

// ReSharper disable once CheckNamespace
namespace Chabot.User
{
    public class UserIdentity
    {
        public UserIdentity(string id, bool isAuthenticated, string? stateKey, IReadOnlyList<Claim> claims)
        {
            Id = id;
            IsAuthenticated = isAuthenticated;
            StateKey = stateKey;
            Claims = claims;
        }

        public string Id { get; }

        public bool IsAuthenticated { get; }

        public string? StateKey { get; }

        public IReadOnlyList<Claim> Claims { get; }
    }
}