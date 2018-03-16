using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestMakerFreeWebApp.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestMakerFreeWebApp.Controllers {
  [Route("api/[controller]")]
    public class BaseApiController : Controller
    {
    public BaseApiController(ApplicationDbContext dbContext)
    {
      DbContext = dbContext;
      JsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
    }

    protected  ApplicationDbContext DbContext { get; private set; }
    protected JsonSerializerSettings JsonSettings { get; private set; }
  }
}
