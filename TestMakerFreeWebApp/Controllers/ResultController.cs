using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMakerFreeWebApp.ViewModels;

namespace TestMakerFreeWebApp.Controllers
{
    [Route("api/[controller]")]
    public class ResultController : Controller
    {
        [HttpGet("All/{quizId}")]
        public IActionResult All(int quizId)
        {
            var sampleResults = new List<ResultViewModel>();
            // add a first sample result
            sampleResults.Add(new ResultViewModel()
            {
                Id = 1,
                QuizId = quizId,
                Text = "What do you value most in your life?",
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now
            });

            for (int i = 2; i <= 5; i++)
            {
                sampleResults.Add(new ResultViewModel()
                {
                    Id = i,
                    QuizId = quizId,
                    Text = String.Format("Sample Question {0}", i),
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                });
            }

            return new JsonResult(sampleResults,
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
            return Content("Not implemented (yet)!");
        }
        /// <summary>
        /// Adds a new Answer to the Database
        /// </summary>
        /// <param name="m">The AnswerViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put(ResultViewModel m)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Edit the Answer with the given {id}
        /// </summary>
        /// <param name="m">The AnswerViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post(ResultViewModel m)
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
