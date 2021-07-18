using System;

// ReSharper disable once CheckNamespace
namespace Chabot.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute
    {
        public AuthorizeAttribute(params string[] roles)
        {
            Roles = roles;
        }

        public string[] Roles { get; }
    }
}