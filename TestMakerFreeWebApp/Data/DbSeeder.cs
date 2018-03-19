using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestMakerFreeWebApp.Data.Models;

namespace TestMakerFreeWebApp.Data {
  public class DbSeeder
    {
        public static void Seed(ApplicationDbContext dbContext,
          RoleManager<IdentityRole> roleManager,
          UserManager<ApplicationUser> userManager)
        {
            if (!dbContext.Users.Any())
            {
        CreateUsers(dbContext, roleManager, userManager)
  .GetAwaiter()
  .GetResult();
            }
            if (!dbContext.Quizzes.Any())
            {
                CreateQuizzes(dbContext);
            }
        }

        private static void CreateQuizzes(ApplicationDbContext dbContext)
        {
            // local variables
            var createdDate = new DateTime(2016, 03, 01, 12, 30, 00);
            DateTime lastModifiedDate = DateTime.Now;
      // retrieve the admin user, which we'll use as default author.
      string authorId = dbContext.Users
            .Where(u => u.UserName == "Admin")
            .FirstOrDefault()
            .Id;
#if DEBUG
      // create 47 sample quizzes with auto-generated data
      // (including questions, answers & results)
      int num = 47;
            for (int i = 1; i <= num; i++)
            {
                CreateSampleQuiz(
                dbContext,
                i,
                authorId,
                num - i,
                3,
                3,
                3,
                createdDate.AddDays(-num));
            }
#endif

            // create 3 more quizzes with better descriptive data
            // (we'll add the questions, answers & results later on)
            EntityEntry<Quiz> e1 = dbContext.Quizzes.Add(new Quiz()
            {
                UserId = authorId,
                Title = "Are you more Light or Dark side of the Force?",
                Description = "Star Wars personality test",
                Text = @"Choose wisely you must, young padawan: " +
            "this test will prove if your will is strong enough " +
            "to adhere to the principles of the light side of the Force " +
            "or if you're fated to embrace the dark side. " +
            "No you want to become a true JEDI, you can't possibly  miss this!",
                ViewCount = 2343,
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            });

            EntityEntry<Quiz> e2 = dbContext.Quizzes.Add(new Quiz()
            {
                UserId = authorId,
                Title = "GenX, GenY or Genz?",
                Description = "Find out what decade most represents you",
                Text = @"Do you feel confortable in your generation? " +
"What year should you have been born in?" +
"Here's a bunch of questions that will help you to find out!",
                ViewCount = 4180,
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            });

            EntityEntry<Quiz> e3 = dbContext.Quizzes.Add(new Quiz()
            {
                UserId = authorId,
                Title = "Which Shingeki No Kyojin character are you?",
                Description = "Attack On Titan personality test",
                Text = @"Do you relentlessly seek revenge like Eren? " +
"Are you willing to put your like on the stake to protect your friends like Mikasa ? " +
"Would you trust your fighting skills like Levi " +
"or rely on your strategies and tactics like Arwin? " +
"Unveil your true self with this Attack On Titan personality test!",
                ViewCount = 5203,
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            });
            dbContext.SaveChanges();
        }

        private static void CreateSampleQuiz(ApplicationDbContext dbContext,
            int num,
            string authorId,
            int viewCount,
            int numberOfQuestions,
            int numberOfAnswersPerQuestion,
            int numberOfResults,
            DateTime createdDate)
        {
            var quiz = new Quiz
            {
                UserId = authorId,
                Title = $"Quiz {num} Title",
                Description = $"This is a sample description for quiz {num}.",
                Text = $"This is a sample quiz created by the DbSeeder class for testing purpoase. All the questions, answers  and results are auto-generated as well {num}",
                ViewCount = viewCount,
                CreatedDate = createdDate,
                LastModifiedDate = createdDate
            };

            dbContext.Quizzes.Add(quiz);
            dbContext.SaveChanges();


            for (int i = 0; i < numberOfQuestions; i++)
            {
                var question = new Question()
                {
                    QuizId = quiz.Id,
                    Text = "This is a sample question created by the DbSeeder class for testing purposes. " +
                    "All the child answers are auto-generated as well.",
                    CreatedDate = createdDate,
                    LastModifiedDate = createdDate
                };
                dbContext.Questions.Add(question);
                dbContext.SaveChanges();
                for (int i2 = 0; i2 < numberOfAnswersPerQuestion; i2++)
                {
                    var e2 = dbContext.Answers.Add(new Answer()
                    {
                        QuestionId = question.Id,
                        Text = "This is a sample answer created by the DbSeeder class for testing purposes. ",
                        Value = i2,
                        CreatedDate = createdDate,
                        LastModifiedDate = createdDate
                    });
                }
            }
            for (int i = 0; i < numberOfResults; i++)
            {
                dbContext.Results.Add(new Result()
                {
                    QuizId = quiz.Id,
                    Text = "This is a sample result created by the DbSeeder class for testing purposes. ",
                    MinValue = 0,
                    // max value should be equal to answers number * max answer value
                    MaxValue = numberOfAnswersPerQuestion * 2,
                    CreatedDate = createdDate,
                    LastModifiedDate = createdDate
                });
            }
            dbContext.SaveChanges();
        }
    

        private static async Task  CreateUsers(ApplicationDbContext dbContext,
          RoleManager<IdentityRole> roleManager,
UserManager<ApplicationUser> userManager)
        {
            var createdDate = new DateTime(2016, 03, 01, 12, 30, 00);
            DateTime lastModifiedDate = DateTime.Now;

      string role_Administrator = "Administrator";
      string role_RegisteredUser = "RegisteredUser";

      if (!await roleManager.RoleExistsAsync(role_Administrator)) {
        await roleManager.CreateAsync(new IdentityRole(role_Administrator));
      }
      if (!await roleManager.RoleExistsAsync(role_RegisteredUser)) {
        await roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
      }

            var userAdmin = new ApplicationUser
            {
              SecurityStamp = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString(),
                UserName = "Admin",
                Email = "admin@example.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };
      if (await userManager.FindByNameAsync(userAdmin.UserName) == null) {
        await userManager.CreateAsync(userAdmin, "Pass4Admin");
        await userManager.AddToRoleAsync(userAdmin, role_RegisteredUser);
        await userManager.AddToRoleAsync(userAdmin, role_Administrator);
        userAdmin.EmailConfirmed = true;
        userAdmin.LockoutEnabled = false;
      }
            //dbContext.Users.Add(userAdmin);

#if DEBUG
            // Create some sample registered user accounts (if they don't exist            already)
            var user_Ryan = new ApplicationUser()
            {
              SecurityStamp = Guid.NewGuid().ToString(),
               // Id = Guid.NewGuid().ToString(),
                UserName = "Ryan",
                Email = "ryan@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };
            var user_Solice = new ApplicationUser()
            {
              SecurityStamp = Guid.NewGuid().ToString(),
             // Id = Guid.NewGuid().ToString(),
                UserName = "Solice",
                Email = "solice@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };
            var user_Vodan = new ApplicationUser()
            {
              SecurityStamp = Guid.NewGuid().ToString(),
              // Id = Guid.NewGuid().ToString(),
              UserName = "Vodan",
                Email = "vodan@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };
      if (await userManager.FindByNameAsync(user_Ryan.UserName) == null) {
        await userManager.CreateAsync(user_Ryan, "Pass4Ryan");
        await userManager.AddToRoleAsync(user_Ryan, role_RegisteredUser);
        user_Ryan.EmailConfirmed = true;
        user_Ryan.LockoutEnabled = false;
      }
      if (await userManager.FindByNameAsync(user_Solice.UserName) == null) {
        await userManager.CreateAsync(user_Solice, "Pass4Solice");
        await userManager.AddToRoleAsync(user_Solice, role_RegisteredUser);
        user_Solice.EmailConfirmed = true;
        user_Solice.LockoutEnabled = false;
      }
      if (await userManager.FindByNameAsync(user_Vodan.UserName) == null) {
        await userManager.CreateAsync(user_Vodan, "Pass4Vodan");
        await userManager.AddToRoleAsync(user_Vodan, role_RegisteredUser);
        user_Vodan.EmailConfirmed = true;
        user_Vodan.LockoutEnabled = false;
      }
            // Insert sample registered users into the Database
           // dbContext.Users.AddRange(user_Ryan, user_Solice, user_Vodan);
#endif
         await    dbContext.SaveChangesAsync();
        }
    }
}
