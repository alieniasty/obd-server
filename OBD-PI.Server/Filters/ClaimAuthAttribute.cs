using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OBDPI.Server.Filters
{
    public class ClaimAuthAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _value;
        private readonly string _type;

        public ClaimAuthAttribute(string type, string value)
        {
            _type = type;
            _value = value;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ","");

            if (string.IsNullOrEmpty(authHeader))
            {
                context.Result = new ForbidResult();
                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(authHeader) as JwtSecurityToken;

            if (token == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasClaim = token.Claims.Any(c => c.Type == _type && c.Value == _value);
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
