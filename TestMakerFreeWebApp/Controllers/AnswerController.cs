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
  public class AnswerController : BaseApiController
  {

    public AnswerController(ApplicationDbContext context,
      RoleManager<IdentityRole> roleManager,
      UserManager<ApplicationUser> userManager,
      IConfiguration configuration) : base(context, roleManager, userManager, configuration)
    {
    }
    [HttpGet("All/{questionId}")]
    public IActionResult All(int questionId)
    {
      var answers = DbContext.Answers.Where(q => q.QuestionId == questionId).ToArray();

      return new JsonResult(
          answers.Adapt<AnswerViewModel[]>(),JsonSettings
          );

    }
    /// <summary>
    /// Retrieves the Answer with the given {id}
    /// </summary>
    /// &lt;param name="id">The ID of an existing Answer</param>
    /// <returns>the Answer with the given {id}</returns>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
      var answer = DbContext.Answers.FirstOrDefault(i => i.Id == id);
      if (answer == null)
      {
        return NotFound(new
        {
          Error = $"Answer with ID {id} not found."
        });
      }
      return Json(answer.Adapt<AnswerViewModel>(), JsonSettings);
    }
    /// <summary>
    /// Adds a new Answer to the Database
    /// </summary>
    /// <param name="m">The AnswerViewModel containing the data to insert</param>
    [HttpPut]
    public IActionResult Put([FromBody]AnswerViewModel model)
    {
      if (model == null)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
      var answer = model.Adapt<Answer>();

      answer.QuestionId = model.QuestionId;
      answer.Text = model.Text;
      answer.Notes = model.Notes;

      answer.CreatedDate = DateTime.Now;
      answer.LastModifiedDate = answer.CreatedDate;

      DbContext.Answers.Add(answer);
      DbContext.SaveChanges();

      return Json(answer.Adapt<AnswerViewModel>(), JsonSettings);
    }
    /// <summary>
    /// Edit the Answer with the given {id}
    /// </summary>
    /// <param name="m">The AnswerViewModel containing the data to update</param>
    [HttpPost]
    public IActionResult Post([FromBody] AnswerViewModel model)
    {
      if (model == null)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }

      var answer = DbContext.Answers.FirstOrDefault(x => x.Id == model.Id);

      if (answer == null)
      {
        return NotFound(new
        {
          Error = $"Answer with ID {model.Id} not found."
        });
      }

      answer.QuestionId = model.QuestionId;
      answer.Text = model.Text;
      answer.Notes = model.Notes;
      answer.Value = model.Value;

      answer.LastModifiedDate = answer.CreatedDate;

      DbContext.SaveChanges();


      return Json(answer.Adapt<AnswerViewModel>(), JsonSettings);
    }
    /// <summary>
    /// Deletes the Answer with the given {id} from the Database
    /// </summary>
    /// <param name="id">The ID of an existing Answer</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var answer = DbContext.Answers.FirstOrDefault(i => i.Id == id);
      if (answer == null)
      {
        return NotFound(new
        {
          Error = $"Answer with ID {id} not found."
        });
      }
      DbContext.Answers.Remove(answer);
      DbContext.SaveChanges();

      return Ok();
    }
  }
}
