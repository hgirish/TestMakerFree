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
  public class ResultController : Controller
  {
    private readonly ApplicationDbContext _dbContext;

    public ResultController(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }
    [HttpGet("All/{quizId}")]
    public IActionResult All(int quizId)
    {
      var answers = _dbContext.Results.Where(q => q.QuizId == quizId).ToArray();

      return new JsonResult(answers.Adapt<QuizViewModel[]>());

    }
    /// <summary>
    /// Retrieves the Answer with the given {id}
    /// </summary>
    /// &lt;param name="id">The ID of an existing Answer</param>
    /// <returns>the Answer with the given {id}</returns>
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
      var result = _dbContext.Results.FirstOrDefault(i => i.Id == id);

      if (result == null)
      {
        return NotFound(new { Error = $"Result ID {id} has not been found." });
      }

      return Json(result.Adapt<ResultViewModel>());
    }
    /// <summary>
    /// Adds a new Answer to the Database
    /// </summary>
    /// <param name="m">The AnswerViewModel containing the data to insert</param>
    [HttpPut]
    public IActionResult Put(ResultViewModel model)
    {
      if (model == null )
      {
        return StatusCode(StatusCodes.Status500InternalServerError);
      }

      var result = model.Adapt<Result>();

      result.CreatedDate = DateTime.Now;
      result.LastModifiedDate = result.CreatedDate;

      _dbContext.Results.Add(result);
      _dbContext.SaveChanges();

      return Json(result.Adapt<ResultViewModel>());


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


      var result = _dbContext.Results.FirstOrDefault(i => i.Id == model.Id);

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

      _dbContext.SaveChanges();


      return Json(result.Adapt<ResultViewModel>());
    }
    /// <summary>
    /// Deletes the Answer with the given {id} from the Database
    /// </summary>
    /// <param name="id">The ID of an existing Answer</param>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      var result = _dbContext.Results.FirstOrDefault(i => i.Id == id);
      if (result == null)
      {
        return NotFound(new
        {
          Error = $"Result with ID {id} not found."
        });
      }
      _dbContext.Results.Remove(result);
      _dbContext.SaveChanges();

      return Ok();
    }
  }
}
