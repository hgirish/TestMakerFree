using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using TestMakerFreeWebApp.ViewModels;

namespace TestMakerFreeWebApp.Controllers
{
  [Route("api/[controller]")]
  public class QuizController : BaseApiController
  {

    public QuizController(ApplicationDbContext context) :base(context)
    {
    }
    /// <summary>
    /// Retrieves the Quiz with the given {id}
    /// </summary>
    /// <param name="id">The ID of an existing Quiz</param>
    /// <returns>the Quiz with the given {id}</returns>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
      var quiz = DbContext.Quizzes.Where(x => x.Id == id).FirstOrDefault();
      if (quiz == null)
      {
        return NotFound(new
        {
          Error = $"Quiz ID {id} has not been found"
        });
      }



      return new JsonResult(quiz.Adapt<QuizViewModel>(), JsonSettings);
    }

    [HttpGet("Latest/{num:int?}")]
    public IActionResult Latest(int num = 10)
    {
      var latest = DbContext.Quizzes
          .OrderByDescending(q => q.CreatedDate)
          .Take(num)
          .ToArray();

      return new JsonResult(latest.Adapt<QuizViewModel[]>(),
          JsonSettings);

    }

    [HttpGet("ByTitle/{num:int?}")]
    public IActionResult ByTitle(int num = 10)
    {
      var byTitle = DbContext.Quizzes
          .OrderBy(q => q.Title)
          .Take(num)
          .ToArray();

      return new JsonResult(
          byTitle.Adapt<QuizViewModel[]>(),
         JsonSettings);
    }

    [HttpGet("Random/{num:int?}")]
    public IActionResult Random(int num = 10)
    {
      var randomQuizzed = DbContext.Quizzes
          .OrderBy(q => Guid.NewGuid())
          .Take(num)
          .ToArray();

      return new JsonResult(
          randomQuizzed.Adapt<QuizViewModel[]>(),
          JsonSettings);
    }


    /// <summary>
    /// Adds a new Quiz to the Database
    /// </summary>
    /// <param name="m">The QuizViewModel containing the data to insert</param>
    [HttpPut]
    public IActionResult Put([FromBody]QuizViewModel model)
    {
      // return a generic HTTP Status 500 (Server Error)
      // if the client payload is invalid.
      if (model == null)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }

      var quiz = new Quiz {
        Title = model.Title,
        Description = model.Description,
        Text = model.Text,
        Notes = model.Notes,

        CreatedDate = DateTime.Now
      };
      quiz.LastModifiedDate = quiz.CreatedDate;

      quiz.UserId = DbContext.Users.Where(u => u.UserName == "Admin").FirstOrDefault().Id;

      DbContext.Quizzes.Add(quiz);

      DbContext.SaveChanges();

      return new JsonResult(quiz.Adapt<QuizViewModel>(),
       JsonSettings);

    }
    /// <summary>
    /// Edit the Quiz with the given {id}
    /// </summary>
    /// <param name="m">The Quiz containing the data to update</param>
    [HttpPost]
    public IActionResult Post([FromBody] QuizViewModel model)
    {
      if (model == null)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }

      var quiz = DbContext.Quizzes.Where(q => q.Id == model.Id).FirstOrDefault();

      if (quiz == null)
      {
        return NotFound(new
        {
          Error = $"Quiz ID {model.Id} has not been found."
        });
      }

      quiz.Title = model.Title;
      quiz.Description = model.Description;
      quiz.Text = model.Text;
      quiz.Notes = model.Notes;

      quiz.LastModifiedDate = quiz.CreatedDate;

      DbContext.SaveChanges();

      return new JsonResult(quiz.Adapt<QuizViewModel>(),
        JsonSettings);
    }
    /// <summary>
    /// Deletes the Quiz with the given {id} from the Database
    /// </summary>
    /// <param name="id">The ID of an existing Quiz</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var quiz = DbContext.Quizzes.Where(q => q.Id == id).FirstOrDefault();

      if (quiz == null)
      {
        return NotFound(new { Error = $"Quiz ID {id} has not been found." });
      }
      DbContext.Quizzes.Remove(quiz);
      DbContext.SaveChanges();

      return Ok();
    }
  }
}
