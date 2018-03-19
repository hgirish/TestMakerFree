using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TestMakerFreeWebApp.ViewModels;
using Mapster;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace TestMakerFreeWebApp.Controllers {
  [Route("api/[controller]")]
  public class ResultController : BaseApiController
  {


    public ResultController(ApplicationDbContext context,
      RoleManager<IdentityRole> roleManager,
      UserManager<ApplicationUser> userManager,
      IConfiguration configuration) : base(context, roleManager, userManager, configuration)
    {
     
    }
    [HttpGet("All/{quizId}")]
    public IActionResult All(int quizId)
    {
      var answers = DbContext.Results.Where(q => q.QuizId == quizId).ToArray();

      return new JsonResult(answers.Adapt<ResultViewModel[]>(),JsonSettings);

    }
    /// <summary>
    /// Retrieves the Answer with the given {id}
    /// </summary>
    /// &lt;param name="id">The ID of an existing Answer</param>
    /// <returns>the Answer with the given {id}</returns>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
      var result = DbContext.Results.FirstOrDefault(i => i.Id == id);

      if (result == null)
      {
        return NotFound(new { Error = $"Result ID {id} has not been found." });
      }

      return Json(result.Adapt<ResultViewModel>(), JsonSettings);
    }
    /// <summary>
    /// Adds a new Answer to the Database
    /// </summary>
    /// <param name="m">The AnswerViewModel containing the data to insert</param>
    [HttpPut]
    public IActionResult Put([FromBody]ResultViewModel model)
    {
      if (model == null )
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }

      var result = model.Adapt<Result>();

      result.CreatedDate = DateTime.Now;
      result.LastModifiedDate = result.CreatedDate;

      DbContext.Results.Add(result);
      DbContext.SaveChanges();

      return Json(result.Adapt<ResultViewModel>(), JsonSettings);


    }
    /// <summary>
    /// Edit the Answer with the given {id}
    /// </summary>
    /// <param name="m">The AnswerViewModel containing the data to update</param>
    [HttpPost]
    public IActionResult Post([FromBody] ResultViewModel model)
    {
      if (model == null)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }


      var result = DbContext.Results.FirstOrDefault(i => i.Id == model.Id);

      if (result == null)
      {
        return NotFound(new { Error = $"Result ID {model.Id} has not been found." });
      }
      result.QuizId = model.QuizId;
      result.Text = model.Text;
      result.MinValue = model.MinValue;
      result.MaxValue = model.MaxValue;
      result.Notes = model.Notes;

      result.LastModifiedDate = result.CreatedDate;

      DbContext.SaveChanges();


      return Json(result.Adapt<ResultViewModel>(), JsonSettings);
    }
    /// <summary>
    /// Deletes the Answer with the given {id} from the Database
    /// </summary>
    /// <param name="id">The ID of an existing Answer</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var result = DbContext.Results.FirstOrDefault(i => i.Id == id);
      if (result == null)
      {
        return NotFound(new
        {
          Error = $"Result with ID {id} not found."
        });
      }
      DbContext.Results.Remove(result);
      DbContext.SaveChanges();

      return Ok();
    }
  }
}
