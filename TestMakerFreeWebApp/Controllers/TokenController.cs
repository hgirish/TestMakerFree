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

namespace TestMakerFreeWebApp.Controllers
{
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
        default:
          return Unauthorized();
      }
    }
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
        DateTime now = DateTime.UtcNow;
        var claims = new[] {
          new Claim(JwtRegisteredClaimNames.Sub, user.Id),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(JwtRegisteredClaimNames.Iat,
          new DateTimeOffset(now).ToUnixTimeSeconds().ToString())
        };

        int tokenExpirationMins =
          Configuration.GetValue<int>("Auth:Jwt:TokenExpirationInMinutes");
        var issuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(Configuration["Auth:Jwt:Key"]));

        var token = new JwtSecurityToken(
          issuer: Configuration["Auth:Jwt:Issuer"],
          audience: Configuration["Auth:Jwt:Audience"],
          claims: claims,
          notBefore: now,
          expires: now.Add(TimeSpan.FromMinutes(tokenExpirationMins)),
          signingCredentials: new SigningCredentials(
            issuerSigningKey,SecurityAlgorithms.HmacSha256)
          );

        string encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        var response = new TokenResponseViewModel {
          token = encodedToken,
          expiration = tokenExpirationMins
        };
        return Json(response);

      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
        return Unauthorized();
      }
    }
  }
}
