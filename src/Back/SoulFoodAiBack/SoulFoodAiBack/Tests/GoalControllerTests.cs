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
    public class GoalControllerTests
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
        public async Task GetAllGoals_DeberiaDevolverOk_ConListaDeObjetivos()
        {
            var dbContext = await GetDatabaseContext();
            dbContext.Goals.Add(new Goal { GoalName = "Bajar peso", Description = "Testá1" });
            await dbContext.SaveChangesAsync();
            var controller = new GoalController(dbContext);

            var result = await controller.GetAllGoals();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var goals = Assert.IsType<List<GoalDto>>(okResult.Value);
            Assert.Single(goals);
            Assert.Equal("Bajar peso", goals[0].GoalName);
        }

        [Fact]
        public async Task AddGoal_DeberiaDevolverOk_YGuardarEnBaseDeDatos()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new GoalController(dbContext);
            var newGoalDto = new CreateGoalDto { GoalName = "Ganar Músculo", Description = "Testá2" };

            var result = await controller.AddGoal(newGoalDto);

            Assert.IsType<OkResult>(result);
            var goalInDb = await dbContext.Goals.FirstOrDefaultAsync(g => g.GoalName == "Ganar Músculo");
            Assert.NotNull(goalInDb);
        }

        [Fact]
        public async Task DeleteGoal_DeberiaDevolverNotFound_SiNoExiste()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new GoalController(dbContext);

            var result = await controller.DeleteGoal(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Ese objetivo no existe.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteGoal_DeberiaBorrar_SiExiste()
        {
            var dbContext = await GetDatabaseContext();
            var goal = new Goal { GoalName = "Borrar", Description = "Test" };
            dbContext.Goals.Add(goal);
            await dbContext.SaveChangesAsync();
            var controller = new GoalController(dbContext);

            var result = await controller.DeleteGoal(goal.IdGoal);

            Assert.IsType<OkObjectResult>(result);
            var goalsLeft = await dbContext.Goals.ToListAsync();
            Assert.Empty(goalsLeft);
        }

        [Fact]
        public async Task EditGoal_DeberiaDevolverNotFound_SiNoExiste()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new GoalController(dbContext);
            var updateDto = new GoalDto { IdGoal = 99, GoalName = "Fallo", Description = "Fallo" };

            var result = await controller.EditGoal(updateDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Objetivo no existe en la base de datos.", notFoundResult.Value);
        }

        [Fact]
        public async Task EditGoal_DeberiaActualizar_SiExiste()
        {
            var dbContext = await GetDatabaseContext();
            var goal = new Goal { GoalName = "Original", Description = "Test" };
            dbContext.Goals.Add(goal);
            await dbContext.SaveChangesAsync();
            var controller = new GoalController(dbContext);
            var updateDto = new GoalDto { IdGoal = goal.IdGoal, GoalName = "Editado", Description = "TestáEditado" };

            var result = await controller.EditGoal(updateDto);

            Assert.IsType<OkResult>(result);
            var updatedGoal = await dbContext.Goals.FindAsync(goal.IdGoal);
            Assert.Equal("Editado", updatedGoal.GoalName);
        }
    }
}