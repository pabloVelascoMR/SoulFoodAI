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
    public class UserDiaryControllerTests
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
        public async Task SubmitReport_DeberiaDevolverNotFound_SiNoExisteUsuario()
        {
            var db = await GetDatabaseContext();
            var controller = new UserDiaryController(db, GetConfig());
            var dto = new WeeklyReportDto { IdUser = 99 };
            var result = await controller.SubmitReportAndCreatePlan(dto);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SubmitReport_DeberiaDevolverOk_YCrearPlanClasico()
        {
            var db = await GetDatabaseContext();
            db.FoodPlans.Add(new FoodPlan { IdFoodPlan = 1, FoodPlanName = "Plan", ProteinPercent = 0.3f, CarbPercent = 0.4f, FatPercent = 0.3f });
            db.Goals.Add(new Goal { IdGoal = 1, GoalName = "Bajar" });
            db.UserDatas.Add(new UserData { IdUser = 1, Age = 25, Gender = "Hombre", Weight = 80f, Height = 1.80f, IdFoodPlan = 1, IdGoal = 1 });
            db.UserFoodPlansWeek.Add(new UserFoodPlanWeek { IdUserFoodPlanWeek = 1, IdUser = 1, IsActive = true });
            await db.SaveChangesAsync();

            var controller = new UserDiaryController(db, GetConfig());
            var dto = new WeeklyReportDto
            {
                IdUser = 1,
                UseAiAdjustment = false,
                HungerLevel = 5,
                EnergyLevel = 5,
                SleepQuality = 5,
                DietAdherence = 8,
                NewWeight = 79f,
                NewMeasures = new UpdateBodyMeasuresDto { ChestMeasure = 100f }
            };

            var result = await controller.SubmitReportAndCreatePlan(dto);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}