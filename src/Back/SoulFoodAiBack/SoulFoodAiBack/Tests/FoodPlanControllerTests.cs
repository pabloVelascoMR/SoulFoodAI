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
    public class FoodPlanControllerTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact]
        public async Task GetAllFoodPlan_DeberiaDevolverOk()
        {
            var dbContext = await GetDatabaseContext();
            dbContext.FoodPlans.Add(new FoodPlan { FoodPlanName = "Keto", Description = "Bajo en carbohidratos" });
            await dbContext.SaveChangesAsync();
            var controller = new FoodPlanController(dbContext);

            var result = await controller.GetAllFoodPlan();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var foodPlans = Assert.IsType<List<FoodPlanDto>>(okResult.Value);
            Assert.Single(foodPlans);
            Assert.Equal("Keto", foodPlans[0].FoodPlanName);
        }

        [Fact]
        public async Task AddFoodPlan_DeberiaDevolverOk_YGuardar()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new FoodPlanController(dbContext);
            var newPlan = new CreateFoodPlanDto { FoodPlanName = "Vegano", Description = "Sin origen animal" };

            var result = await controller.AddFoodPlan(newPlan);

            Assert.IsType<OkResult>(result);
            var planInDb = await dbContext.FoodPlans.FirstOrDefaultAsync(p => p.FoodPlanName == "Vegano");
            Assert.NotNull(planInDb);
        }

        [Fact]
        public async Task DeleteFoodPlan_DeberiaDevolverNotFound_SiNoExiste()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new FoodPlanController(dbContext);

            var result = await controller.DeleteFoodPlan(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Ese plan no existe.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteFoodPlan_DeberiaBorrar_SiExiste()
        {
            var dbContext = await GetDatabaseContext();
            var plan = new FoodPlan { FoodPlanName = "Borrar", Description = "Test" };
            dbContext.FoodPlans.Add(plan);
            await dbContext.SaveChangesAsync();
            var controller = new FoodPlanController(dbContext);

            var result = await controller.DeleteFoodPlan(plan.IdFoodPlan);

            Assert.IsType<OkObjectResult>(result);
            var plansLeft = await dbContext.FoodPlans.ToListAsync();
            Assert.Empty(plansLeft);
        }

        [Fact]
        public async Task EditFoodPlan_DeberiaDevolverNotFound_SiNoExiste()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new FoodPlanController(dbContext);
            var updateDto = new FoodPlanDto { IdFoodPlan = 99, FoodPlanName = "Error", Description = "Error" };

            var result = await controller.EditFoodPlan(updateDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Plan no existe en la base de datos.", notFoundResult.Value);
        }

        [Fact]
        public async Task EditFoodPlan_DeberiaActualizar_SiExiste()
        {
            var dbContext = await GetDatabaseContext();
            var plan = new FoodPlan { FoodPlanName = "Original", Description = "Test" };
            dbContext.FoodPlans.Add(plan);
            await dbContext.SaveChangesAsync();
            var controller = new FoodPlanController(dbContext);
            var updateDto = new FoodPlanDto { IdFoodPlan = plan.IdFoodPlan, FoodPlanName = "Editado", Description = "Test" };

            var result = await controller.EditFoodPlan(updateDto);

            Assert.IsType<OkResult>(result);
            var updatedPlan = await dbContext.FoodPlans.FindAsync(plan.IdFoodPlan);
            Assert.Equal("Editado", updatedPlan.FoodPlanName);
        }
    }
}