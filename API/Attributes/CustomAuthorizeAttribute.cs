using Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ShanEnterprises.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    public CustomAuthorizeAttribute(params UserRole[] roles)
    {
        Roles = Roles = string.Join(",", roles.Select(r => r.ToString()));
    }
}