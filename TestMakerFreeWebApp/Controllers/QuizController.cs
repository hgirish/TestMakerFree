using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.ViewModels;

namespace TestMakerFreeWebApp.Controllers
{
    [Route("api/[controller]")]
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves the Answer with the given {id}
        /// </summary>
        /// &lt;param name="id">The ID of an existing Answer</param>
        /// <returns>the Answer with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var quiz = _context.Quizzes.Where(x => x.Id == id).FirstOrDefault();

            return new JsonResult(quiz.Adapt<QuizViewModel>(),
                new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpGet("Latest/{num:int?}")]
        public IActionResult Latest(int num = 10)
        {
            var latest = _context.Quizzes
                .OrderByDescending(q => q.CreatedDate)
                .Take(num)
                .ToArray();

            return new JsonResult(latest.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });

        }

        [HttpGet("ByTitle/{num:int?}")]
        public IActionResult ByTitle(int num = 10)
        {
            var byTitle = _context.Quizzes
                .OrderBy(q => q.Title)
                .Take(num)
                .ToArray();

            return new JsonResult(
                byTitle.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });
        }

        [HttpGet("Random/{num:int?}")]
        public IActionResult Random(int num = 10)
        {
            var randomQuizzed = _context.Quizzes
                .OrderBy(q => Guid.NewGuid())
                .Take(num)
                .ToArray();

            return new JsonResult(
                randomQuizzed.Adapt<QuizViewModel[]>(),
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });
        }
     
    
        /// <summary>
        /// Adds a new Answer to the Database
        /// </summary>
        /// <param name="m">The AnswerViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put(QuizViewModel m)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Edit the Answer with the given {id}
        /// </summary>
        /// <param name="m">The AnswerViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post(QuizViewModel m)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Deletes the Answer with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Answer</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
