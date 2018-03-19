using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestMakerFreeWebApp.Controllers {
  [Route("api/[controller]")]
  public class BaseApiController : Controller {
    public BaseApiController(ApplicationDbContext dbContext,
      RoleManager<IdentityRole> roleManager,
      UserManager<ApplicationUser> userManager,
      IConfiguration configuration)
    {
      DbContext = dbContext;
      RoleManager = roleManager;
      UserManager = userManager;
      Configuration = configuration;
      JsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
    }

    protected ApplicationDbContext DbContext { get; private set; }
    protected RoleManager<IdentityRole> RoleManager { get; private set; }
    protected UserManager<ApplicationUser> UserManager { get; private set; }
    protected IConfiguration Configuration { get; private set; }
    protected JsonSerializerSettings JsonSettings { get; private set; }
  }
}
