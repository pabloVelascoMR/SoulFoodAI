using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SoulFoodAiBack.Controllers;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UserFoodPlanDailyControllerTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new DataContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private IConfiguration GetConfig()
        {
            var inMemorySettings = new Dictionary<string, string> { { "Gemini:ApiKey", "TEST" } };
            return new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        }

        [Fact]
        public async Task GetDailyHeader_DeberiaDevolverNotFound_SiNoExiste()
        {
            var db = await GetDatabaseContext();
            var controller = new UserFoodPlanDailyController(db, GetConfig());
            var result = await controller.GetDailyHeader(99);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetDailyHeader_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.FoodPlans.Add(new FoodPlan { IdFoodPlan = 1, FoodPlanName = "Dieta Prueba" });
            db.UserFoodPlansWeek.Add(new UserFoodPlanWeek { IdUserFoodPlanWeek = 1, IdUser = 1 });
            await db.SaveChangesAsync();

            var day = new UserFoodPlanDaily
            {
                IdUserFoodPlanDaily = 1,
                IdUser = 1,
                IdFoodPlan = 1,             
                IdUserFoodPlanWeek = 1,     
                CreationDate = DateTime.Now,
                TargetKcal = 2000,
                TargetProtein = 100,
                TargetCarbs = 200,
                TargetFat = 50
            };
            db.UserFoodPlansDaily.Add(day);
            await db.SaveChangesAsync();

            var controller = new UserFoodPlanDailyController(db, GetConfig());
            var result = await controller.GetDailyHeader(day.IdUserFoodPlanDaily);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateDailyRecipes_DeberiaDevolverOk_YSumarMacros()
        {
            var db = await GetDatabaseContext();
            db.UserFoodPlansDaily.Add(new UserFoodPlanDaily { IdUserFoodPlanDaily = 1, IdUser = 1 });
            db.Recipes.Add(new Recipe { IdRecipe = 10, RecipeName = "Pollo", TotalKcal = 500, Protein = 40, Carbs = 10, Fat = 5 });
            await db.SaveChangesAsync();

            var controller = new UserFoodPlanDailyController(db, GetConfig());
            var dto = new UpdateDailyRecipesDto { IdUserFoodPlanDaily = 1, RecipeIds = new List<int> { 10 } };
            var result = await controller.UpdateDailyRecipes(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AdjustDayMacros_DeberiaDevolverBadRequest_SiNoHayRecetas()
        {
            var db = await GetDatabaseContext();
            db.UserFoodPlansDaily.Add(new UserFoodPlanDaily { IdUserFoodPlanDaily = 1 });
            await db.SaveChangesAsync();
            var controller = new UserFoodPlanDailyController(db, GetConfig());
            var result = await controller.AdjustDayMacros(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}