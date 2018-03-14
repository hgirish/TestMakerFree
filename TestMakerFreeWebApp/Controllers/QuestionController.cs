using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using TestMakerFreeWebApp.ViewModels;

namespace TestMakerFreeWebApp.Controllers
{
  [Route("api/[controller]")]
  public class QuestionController : Controller
  {
    private readonly ApplicationDbContext _dbContext;

    public QuestionController(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }
    [HttpGet("All/{quizId}")]
    public IActionResult All(int quizId)
    {
      var questions = _dbContext.Questions.Where(q => q.QuizId == quizId).ToArray();

      return Json(questions.Adapt<QuestionViewModel[]>());
    }
    /// <summary>
    /// Retrieves the Question with the given {id}
    /// </summary>
    /// &lt;param name="id">The ID of an existing Question</param>
    /// <returns>the Question with the given {id}</returns>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
      var question = _dbContext.Questions.Where(i => i.Id == id).FirstOrDefault();

      if (question == null)
      {
        return NotFound(new
        {
          Error = $"Question ID {id} has not been found."
        });
      }

      return Json(question.Adapt<QuestionViewModel>(), new JsonSerializerSettings { Formatting = Formatting.Indented });
    }
    /// <summary>
    /// Adds a new Question to the Database
    /// </summary>
    /// <param name="m">The QuestionViewModel containing the data to insert</param>
    [HttpPut]
    public IActionResult Put(QuestionViewModel model)
    {
      if (model == null )
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }

      var question = model.Adapt<Question>();

      question.QuizId = model.QuizId;
      question.Text = model.Text;
      question.Notes = model.Notes;

      question.CreatedDate = DateTime.Now;
      question.LastModifiedDate = question.CreatedDate;

      _dbContext.Questions.Add(question);

      _dbContext.SaveChanges();

      return Json(question.Adapt<QuestionViewModel>());
    }
    /// <summary>
    /// Edit the Question with the given {id}
    /// </summary>
    /// <param name="m">The QuestionViewModel containing the data to update</param>
    [HttpPost]
    public IActionResult Post([FromBody]QuestionViewModel model)
    {
      if (model == null)
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }
      var question = _dbContext.Questions.FirstOrDefault(x => x.Id == model.Id);

      if (question == null)
      {
        return NotFound(new
        {
          Error = $"Question ID {model.Id} has not been found."
        });
      }

      question.QuizId = model.QuizId;
      question.Text = model.Text;
      question.Notes = model.Notes;

      question.LastModifiedDate = question.CreatedDate;

      _dbContext.SaveChanges();

      return Json(question.Adapt<QuestionViewModel>());
    }
    /// <summary>
    /// Deletes the Question with the given {id} from the Database
    /// </summary>
    /// <param name="id">The ID of an existing Question</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var question = _dbContext.Questions.FirstOrDefault(x => x.Id == id);

      if (question == null)
      {
        return NotFound(new
        {
          Error = $"Question ID {id} has not been found."
        });
      }
      _dbContext.Questions.Remove(question);
      _dbContext.SaveChanges();

      return Ok();

    }
  }
}
