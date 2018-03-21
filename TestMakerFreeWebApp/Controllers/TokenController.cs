using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using TestMakerFreeWebApp.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace TestMakerFreeWebApp.Controllers {
    public class TokenController : BaseApiController {
        public TokenController(ApplicationDbContext dbContext,
          RoleManager<IdentityRole> roleManager,
          UserManager<ApplicationUser> userManager,
          IConfiguration configuration) : base(dbContext, roleManager, userManager, configuration)
        {
        }

        [HttpPost("Auth")]
        public async Task<IActionResult> Jwt([FromBody]TokenRequestViewModel model)
        {
            if (model == null) {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            switch (model.grant_type) {
                case "password":
                    return await GetToken(model);
                case "refresh_token":
                    return await RefreshToken(model);
                default:
                    return Unauthorized();
            }
        }

        private async Task<IActionResult> RefreshToken(TokenRequestViewModel model)
        {
            try {
                var rt = DbContext.Tokens
                  .FirstOrDefault(t => t.ClientId == model.client_id
                  && t.Value == model.refresh_token);

                if (rt == null) {
                    return Unauthorized();
                }

                var user = await UserManager.FindByIdAsync(rt.UserId);

                if (user == null) {
                    return Unauthorized();
                }

                var rtNew = CreateRefreshToken(rt.ClientId, rt.UserId);

                DbContext.Tokens.Remove(rt);
                DbContext.Tokens.Add(rtNew);

                DbContext.SaveChanges();

                var response = CreateAccessToken(rtNew.UserId, rtNew.Value);

                return Json(response);


            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return Unauthorized();
            }
        }

        private TokenResponseViewModel CreateAccessToken(string userId, string refreshToken)
        {
            DateTime now = DateTime.Now;

            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat,
        new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
      };

            int tokenExpirationMins =
        Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
            var issuserSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));

            var token = new JwtSecurityToken(
              issuer: Configuration["Auth:Jwt:Issuer"],
              audience: Configuration["Auth:Jwt:Audience"],
              claims: claims,
              notBefore: now,
              expires: now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
              signingCredentials: new SigningCredentials(
                issuserSigningKey, SecurityAlgorithms.HmacSha256)
              );

            string encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponseViewModel {
                token = encodedToken,
                expiration = tokenExpirationMins,
                refresh_token = refreshToken
            };

        }

        private Token CreateRefreshToken(string clientId, string userId) => new Token {
            ClientId = clientId,
            UserId = userId,
            Type = 0,
            Value = Guid.NewGuid().ToString("N"),
            CreatedDate = DateTime.UtcNow
        };

        private async Task<IActionResult> GetToken(TokenRequestViewModel model)
        {
            try {
                var user = await UserManager.FindByNameAsync(model.username);
                if (user == null && model.username.Contains("@")) {
                    user = await UserManager.FindByEmailAsync(model.username);
                }
                if (user == null || !await UserManager.CheckPasswordAsync(user, model.password)) {
                    return Unauthorized();
                }

                var rt = CreateRefreshToken(model.client_id, user.Id);

                DbContext.Tokens.Add(rt);
                DbContext.SaveChanges();

                var t = CreateAccessToken(user.Id, rt.Value);
                return Json(t);
                //DateTime now = DateTime.UtcNow;
                //var claims = new[] {
                //  new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                //  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //  new Claim(JwtRegisteredClaimNames.Iat,
                //  new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
                //};

                //int tokenExpirationMins =
                //  Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
                //var issuerSigningKey = new SymmetricSecurityKey(
                //  Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));

                //var token = new JwtSecurityToken(
                //  issuer: Configuration["Auth:Jwt:Issuer"],
                //  audience: Configuration["Auth:Jwt:Audience"],
                //  claims: claims,
                //  notBefore: now,
                //  expires: now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
                //  signingCredentials: new SigningCredentials(
                //    issuerSigningKey,SecurityAlgorithms.HmacSha256)
                //  );

                //string encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

                //var response = new TokenResponseViewModel {
                //  token = encodedToken,
                //  expiration = tokenExpirationMins
                //};
                //return Json(response);

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return Unauthorized();
            }
        }
    }
}
