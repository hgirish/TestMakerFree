using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMakerFreeWebApp.ViewModels;

namespace TestMakerFreeWebApp.Controllers
{
    [Route("api/[controller]")]
    public class QuizController : Controller
    {
        /// <summary>
        /// Retrieves the Answer with the given {id}
        /// </summary>
        /// &lt;param name="id">The ID of an existing Answer</param>
        /// <returns>the Answer with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var v = new QuizViewModel
            {
                Id = id,
                Title = $"Sample quiz with id {id}",
                Description = "Not a real quiz: it's just a sample",
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            };
            return new JsonResult(v, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpGet("Latest/{num:int?}")]
        public IActionResult Latest(int num = 10)
        {
            var sampleQuizzes = new List<QuizViewModel>();

            sampleQuizzes.Add(new QuizViewModel()
            {
                Id = 1,
                Title = "Which Shingeki No Kyojin character are you?",
                Description = "Anime-related personality test",
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            });

            for (int i = 2; i <= num; i++)
            {
                sampleQuizzes.Add(new QuizViewModel()
                {
                    Id = i,
                    Title = String.Format("Sample Quiz {0}", i),
                    Description = "This is a sample quiz",
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                });
            }

            return new JsonResult(sampleQuizzes,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });

        }

        [HttpGet("ByTitle/{num:int?}")]
        public IActionResult ByTitle(int num = 10)
        {
            var sampleQuizzes = ((JsonResult)Latest(num)).Value
                as List<QuizViewModel>;

            return new JsonResult(
                sampleQuizzes.OrderBy(t=>t.Title),
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                });
        }

        [HttpGet("Random/{num:int?}")]
        public IActionResult Random(int num = 10)
        {
            var sampleQuizzes = ((JsonResult)Latest(num)).Value
                as List<QuizViewModel>;

            return new JsonResult(
                sampleQuizzes.OrderBy(t => Guid.NewGuid()),
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
