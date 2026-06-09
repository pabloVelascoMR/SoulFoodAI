using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Controllers;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UserFoodPlanWeekControllerTests
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

        [Fact]
        public async Task GenerateWeekPlan_DeberiaDevolverBadRequest_SiYaExisteActivo()
        {
            var db = await GetDatabaseContext();
            db.UserFoodPlansWeek.Add(new UserFoodPlanWeek { IdUser = 1, IsActive = true });
            await db.SaveChangesAsync();
            var controller = new UserFoodPlanWeekController(db);

            var result = await controller.GenerateWeekPlan(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GenerateWeekPlan_DeberiaDevolverNotFound_SiFaltanDatosUser()
        {
            var db = await GetDatabaseContext();
            db.UserDatas.Add(new UserData { IdUser = 1 , Gender= "Hombre" }); 
            await db.SaveChangesAsync();
            var controller = new UserFoodPlanWeekController(db);

            var result = await controller.GenerateWeekPlan(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GenerateWeekPlan_DeberiaDevolverOk_YCrearDias()
        {
            var db = await GetDatabaseContext();
            db.Goals.Add(new Goal { IdGoal = 1, GoalName = "Ganar músculo" });
            db.FoodPlans.Add(new FoodPlan { IdFoodPlan = 1, FoodPlanName = "Plan", ProteinPercent = 0.3f, CarbPercent = 0.4f, FatPercent = 0.3f });
            db.UserDatas.Add(new UserData { IdUser = 1, Age = 25, Gender = "Hombre", Weight = 80f, Height = 1.80f, IdFoodPlan = 1, IdGoal = 1, MealsPerDay = 3 });
            await db.SaveChangesAsync();
            var controller = new UserFoodPlanWeekController(db);

            var result = await controller.GenerateWeekPlan(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetWeeklyHeader_DeberiaDevolverNotFound_SiNoHayPlan()
        {
            var db = await GetDatabaseContext();
            var controller = new UserFoodPlanWeekController(db);
            var result = await controller.GetWeeklyHeader(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetWeeklyHeader_DeberiaDevolverOk_SiHayPlan()
        {
            var db = await GetDatabaseContext();
            db.FoodPlans.Add(new FoodPlan { IdFoodPlan = 1, FoodPlanName = "Test" });
            db.UserFoodPlansWeek.Add(new UserFoodPlanWeek { IdUserFoodPlanWeek = 1, IdUser = 1, IdFoodPlan = 1, IsActive = true, TotalWeeklyKcal = 2000, TotalWeeklyProtein = 100, TotalWeeklyCarbs = 200, TotalWeeklyFat = 50 });
            await db.SaveChangesAsync();
            var controller = new UserFoodPlanWeekController(db);
            var result = await controller.GetWeeklyHeader(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetActiveWeekCalendar_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.UserFoodPlansWeek.Add(new UserFoodPlanWeek { IdUserFoodPlanWeek = 1, IdUser = 1, IsActive = true });
            db.UserFoodPlansDaily.Add(new UserFoodPlanDaily { IdUserFoodPlanDaily = 1, IdUserFoodPlanWeek = 1, IdUser = 1 });
            await db.SaveChangesAsync();
            var controller = new UserFoodPlanWeekController(db);

            var result = await controller.GetActiveWeekCalendar(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetPlanHistory_DeberiaDevolverNotFound_SiVacio()
        {
            var db = await GetDatabaseContext();
            var controller = new UserFoodPlanWeekController(db);
            var result = await controller.GetPlanHistory(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetPlanHistory_RecibeNotFound_CubreCodigo()
        {
            var db = await GetDatabaseContext();
            db.UserFoodPlansWeek.Add(new UserFoodPlanWeek { IdUserFoodPlanWeek = 1, IdUser = 1, IsVisibleInHistory = true, StartDate = DateTime.Now });
            await db.SaveChangesAsync();
            var controller = new UserFoodPlanWeekController(db);
            var result = await controller.GetPlanHistory(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task HidePlanFromHistory_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.UserFoodPlansWeek.Add(new UserFoodPlanWeek { IdUserFoodPlanWeek = 1, IdUser = 1, IsVisibleInHistory = true });
            await db.SaveChangesAsync();
            var controller = new UserFoodPlanWeekController(db);

            var result = await controller.HidePlanFromHistory(1);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}