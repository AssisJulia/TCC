using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TCC_SAMMI.CrossCutting;

public static class SetupJWT
{
    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "etec.pwIII",
                    ValidAudience = "etec.pwIII",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("{ccdc511d-23f0-4a30-995e-ebc3658e901d}"))
                };
            });
    }
}