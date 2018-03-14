using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestMakerFreeWebApp.ViewModels;
using Mapster;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace TestMakerFreeWebApp.Controllers
{
  [Route("api/[controller]")]
  public class AnswerController : Controller
  {
    private readonly ApplicationDbContext _dbContext;

    public AnswerController(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }
    [HttpGet("All/{questionId}")]
    public IActionResult All(int questionId)
    {
      var answers = _dbContext.Answers.Where(q => q.QuestionId == questionId).ToArray();

      return new JsonResult(
          answers.Adapt<AnswerViewModel[]>(),
          new Newtonsoft.Json.JsonSerializerSettings
          {
            Formatting = Newtonsoft.Json.Formatting.Indented
          });

    }
    /// <summary>
    /// Retrieves the Answer with the given {id}
    /// </summary>
    /// &lt;param name="id">The ID of an existing Answer</param>
    /// <returns>the Answer with the given {id}</returns>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
      var answer = _dbContext.Answers.FirstOrDefault(i => i.Id == id);
      if (answer == null)
      {
        return NotFound(new
        {
          Error = $"Answer with ID {id} not found."
        });
      }
      return Json(answer.Adapt<AnswerViewModel>());
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

      _dbContext.Answers.Add(answer);
      _dbContext.SaveChanges();

      return Json(answer.Adapt<AnswerViewModel>());
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

      var answer = _dbContext.Answers.FirstOrDefault(x => x.Id == model.Id);

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

      _dbContext.SaveChanges();


      return Json(answer.Adapt<AnswerViewModel>());
    }
    /// <summary>
    /// Deletes the Answer with the given {id} from the Database
    /// </summary>
    /// <param name="id">The ID of an existing Answer</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var answer = _dbContext.Answers.FirstOrDefault(i => i.Id == id);
      if (answer == null)
      {
        return NotFound(new
        {
          Error = $"Answer with ID {id} not found."
        });
      }
      _dbContext.Answers.Remove(answer);
      _dbContext.SaveChanges();

      return Ok();
    }
  }
}
