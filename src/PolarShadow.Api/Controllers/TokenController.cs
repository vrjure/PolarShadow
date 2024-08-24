using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PolarShadow.Api.Utilities;
using PolarShadow.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PolarShadow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IOptions<JWTOptions> _jwtOptions;
        public TokenController(IOptions<JWTOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }

        [HttpPost]
        public TokenModel GetToken([FromBody] TokenRequestModel tokenRequest)
        {
            if (tokenRequest == null)
            {
                throw new ResultException(ResultCode.ParameterError, "参数错误");
            }

            if (tokenRequest.UserName != _jwtOptions.Value.UserName || tokenRequest.Password != _jwtOptions.Value.Password)
            {
                throw new ResultException(ResultCode.ParameterError, "用户名或密码错误");
            }

            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.PrivateKey)), SecurityAlgorithms.HmacSha256Signature);
            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new List<Claim>
                {
                    new Claim(JWTClaimTypes.ClientId, _jwtOptions.Value.ClientId),
                    new Claim(ClaimTypes.Name, _jwtOptions.Value.UserName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.Value.Expires),
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDesc);
            return new TokenModel { AccessToken = handler.WriteToken(token), Expires =_jwtOptions.Value.Expires };
        }
    }
}
