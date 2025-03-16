using BoothHolder.Common.Configration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
namespace BoothHolder.Extensions.ServiceExtensions
{
    public static class AuthenticationSetup
    {
        public static void AddAuth(this IServiceCollection services)
        {
            var time = int.Parse(AppSettings.app("JwtSettings:ClockSkewMinutes"));
            var Audience = AppSettings.app("JwtSettings:Audience");
            var Issuer = AppSettings.app("JwtSettings:Issuer");
            var SecretKey = AppSettings.app("JwtSettings:SecretKey");
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BoothHolder", Version = "v2" });



                // 添加Bearer认证  
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "前面加Bearer",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,

                });

                // 为API添加Bearer认证需求  
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
               {
                  {
                      new OpenApiSecurityScheme
                      {
                          Reference = new OpenApiReference
                          {
                              Type = ReferenceType.SecurityScheme,
                              Id = "Bearer"
                             },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                       },
                      new List<string>()
                   }
                 });
            });



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,  //是否验证超时  当设置exp和nbf时有效 
                       ValidateIssuerSigningKey = true,  ////是否验证密钥
                       ValidAudience = Audience,//Audience
                       ValidIssuer = Issuer,//Issuer，这两项和登陆时颁发的一致
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),     //拿到SecurityKey
                                                                                                           //缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟                                                                                                            //注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                       ClockSkew = TimeSpan.FromMinutes(time)  //设置过期时间
                   };
               });
        }
    }
}
