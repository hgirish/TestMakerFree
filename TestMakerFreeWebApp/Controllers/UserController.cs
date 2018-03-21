using System;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using TestMakerFreeWebApp.ViewModels;

namespace TestMakerFreeWebApp.Controllers {
  public class UserController : BaseApiController {
    public UserController(ApplicationDbContext dbContext,
      RoleManager<IdentityRole> roleManager,
      UserManager<ApplicationUser> userManager,
      IConfiguration configuration)
      : base(dbContext, roleManager, userManager, configuration)
    {
    }
    [HttpPut]
    public async Task<IActionResult> Add([FromBody]UserViewModel model)
    {
      if (model == null) {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
      ApplicationUser user = await UserManager.FindByNameAsync(model.UserName);
      if (user != null) {
        return BadRequest("Username already exists");
      }

      user = await UserManager.FindByEmailAsync(model.Email);

      if (user != null) {
        return BadRequest("Eamil already exists");
      }

      var now = DateTime.Now;

      user = new ApplicationUser {
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = model.UserName,
        Email = model.Email,
        DisplayName = model.DisplayName,
        CreatedDate = now,
        LastModifiedDate = now
      };

      await UserManager.CreateAsync(user, model.Password);

      await UserManager.AddToRoleAsync(user, "RegisteredUser");

      user.EmailConfirmed = true;
      user.LockoutEnabled = false;

      DbContext.SaveChanges();

      return Json(user.Adapt<UserViewModel>(), JsonSettings);
    }
  }
}
