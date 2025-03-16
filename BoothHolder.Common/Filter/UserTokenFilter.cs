using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using StackExchange.Redis;
using System.Security.Claims;

namespace BoothHolder.Common.Filter
{
    public class UserTokenFilter : IAsyncActionFilter
    {
        private readonly IDatabase _database;
        private readonly string _userredistoken = "usertoken:";


        public UserTokenFilter(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authorizeAttribute = context.ActionDescriptor.EndpointMetadata
                .OfType<AuthorizeAttribute>()
                .FirstOrDefault();
            var allowAnonymousAttribute = context.ActionDescriptor.EndpointMetadata
               .OfType<AllowAnonymousAttribute>()
               .FirstOrDefault();
            // 如果没有 [Authorize] 特性，则跳过 token 验证
            if (authorizeAttribute == null || allowAnonymousAttribute != null)
            {
                await next();
                return;
            }
            // 获取请求头中的 token
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
            {
                // 如果没有 token 或者 token 格式错误，返回 401 未授权
                Log.Warning("没有 token 或者 token 格式错误");
                context.Result = new ObjectResult(new
                {
                    message = "没有 token 或者 token 格式错误"
                })
                {
                    StatusCode = 401 // 设置状态码为 401
                };
                return;
            }

            // 提取 Bearer token
            var actualToken = token.Substring("Bearer ".Length).Trim();

            // 验证 token
            if (!await ValidateTokenAsync(actualToken, context.HttpContext.User.Claims))
            {
                // 如果 token 无效，返回 401 未授权
                Log.Warning("未授权");
                context.Result = new ObjectResult(new
                {
                    message = "Token已过期"
                })
                {
                    StatusCode = 401 // 设置状态码为 401
                };

                return;
            }

            // 通过验证后，继续执行下一步操作
            await next();
        }

        private async Task<bool> ValidateTokenAsync(string actualToken, IEnumerable<Claim> claims)
        {



            // 在这里实现你的 token 验证逻辑，比如验证 JWT token 是否有效
            // 你可以通过访问数据库、Redis 或通过第三方服务来验证 token

            // 示例: 假设验证 token 是否为 "valid_token"（这里只是一个示例，实际验证逻辑应该根据你的需求进行）

            var userNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var redistoken = await _database.StringGetAsync(_userredistoken + userNameClaim.Value);

            if (actualToken.Equals(redistoken)) return true;

            return false;

        }
    }
}
