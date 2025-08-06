using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TaleTrail.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // ⚠️ Optional: Add your JWT logic here if needed
            await _next(context);
        }
    }
}
