using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class UserDataControllerTests
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
        public async Task GetAllUserDatas_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            var controller = new UserDataController(db);
            var result = await controller.GetAllUserDatas();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddUserData_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            var controller = new UserDataController(db);
            var dto = new CreateUserDataDto { IdUser = 1, Age = 30, Gender = "Hombre", Weight = 80f, Height = 180f, IdFoodPlan = 1, IdGoal = 1, IdIntolerances = new List<int>() };
            var result = await controller.AddUserData(dto);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task EditUserData_DeberiaActualizar_SiExiste()
        {
            var db = await GetDatabaseContext();
            db.UserDatas.Add(new UserData { IdUser = 1, Age = 25, Gender = "Hombre", Weight = 80f, Height = 180f, IdFoodPlan = 1, IdGoal = 1 });
            await db.SaveChangesAsync();
            var controller = new UserDataController(db);
            var dto = new UserDataDto { IdUser = 1, Age = 30, Gender = "Hombre", Weight = 85f, Height = 180f, IdFoodPlan = 1, IdGoal = 1, IdIntolerances = new List<int>() };
            var result = await controller.EditUserData(dto);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteUserData_DeberiaBorrar_SiExiste()
        {
            var db = await GetDatabaseContext();
            db.UserDatas.Add(new UserData { IdUser = 1, Gender = "H", IdFoodPlan = 1, IdGoal = 1 });
            await db.SaveChangesAsync();
            var controller = new UserDataController(db);
            var result = await controller.DeleteUserData(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateBodyMeasures_DeberiaActualizar_SiExiste()
        {
            var db = await GetDatabaseContext();
            db.UserDatas.Add(new UserData { IdUser = 1, Gender = "H", IdFoodPlan = 1, IdGoal = 1 });
            await db.SaveChangesAsync();
            var controller = new UserDataController(db);
            var dto = new UpdateBodyMeasuresDto { IdUser = 1, ChestMeasure = 100f };
            var result = await controller.UpdateBodyMeasures(dto);
            Assert.IsType<OkResult>(result);
        }
    }
}