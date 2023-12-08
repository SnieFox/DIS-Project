using Microsoft.IdentityModel.Tokens;

namespace DISProject.Api.Services;

public interface ITokenLifetimeManager
{
    (bool IsSuccess, string ErrorManage) SignOut(SecurityToken securityToken);

    bool ValidateTokenLifetime(DateTime? notBefore,
        DateTime? expires,
        SecurityToken securityToken,
        TokenValidationParameters validationParameters);
}