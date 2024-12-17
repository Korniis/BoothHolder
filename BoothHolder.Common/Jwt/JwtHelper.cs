using BoothHolder.Common.Configration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoothHolder.Common.Jwt
{
    public static class JwtHelper
    {


        public static string CreateToken(List<Claim> claims)
        {
            // 1. 定义需要使用到的Claims

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.app("JwtSettings:SecretKey")));
            var expiresIn = Convert.ToInt32(AppSettings.app("JwtSettings:ClockSkewMinutes"));
            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
               AppSettings.app("JwtSettings:Issuer"),     //Issuer
                AppSettings.app("JwtSettings:Audience"),   //Audience
                claims,                          //Claims,
                DateTime.Now,                    //notBefore
                DateTime.Now.AddMinutes(expiresIn),    //expires
                signingCredentials               //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
        /// <summary>
        /// 解码 JWT token 并返回 Claims
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Claims</returns>
        public static List<Claim> DecodeToken(string token)
        {
            try
            {
                // 创建 JwtSecurityTokenHandler 实例
                var tokenHandler = new JwtSecurityTokenHandler();

                // 解析 JWT token
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // 获取 Claims 并返回
                return jwtToken?.Claims.ToList();
            }
            catch (Exception ex)
            {
                // 处理解析错误，例如 token 格式无效
                Console.WriteLine($"解码 token 失败: {ex.Message}");
                return null;
            }
        }
    }
}
