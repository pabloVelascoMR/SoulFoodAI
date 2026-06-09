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
    public class MealControllerTests
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
        public async Task GetAllMeals_DeberiaDevolverOk()
        {
            var dbContext = await GetDatabaseContext();
            dbContext.Meals.Add(new Meal { MealName = "Desayuno" });
            await dbContext.SaveChangesAsync();
            var controller = new MealController(dbContext);

            var result = await controller.GetAllMeals();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var meals = Assert.IsType<List<MealDto>>(okResult.Value);
            Assert.Single(meals);
            Assert.Equal("Desayuno", meals[0].MealName);
        }

        [Fact]
        public async Task AddMeal_DeberiaDevolverOk_YGuardar()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new MealController(dbContext);
            var newDto = new CreateMealDto { MealName = "Cena" };

            var result = await controller.AddMeal(newDto);

            Assert.IsType<OkResult>(result);
            var inDb = await dbContext.Meals.FirstOrDefaultAsync(m => m.MealName == "Cena");
            Assert.NotNull(inDb);
        }

        [Fact]
        public async Task DeleteMeal_DeberiaDevolverNotFound_SiNoExiste()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new MealController(dbContext);

            var result = await controller.DeleteMeal(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Esa comida no existe.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteMeal_DeberiaBorrar_SiExiste()
        {
            var dbContext = await GetDatabaseContext();
            var meal = new Meal { MealName = "Borrar" };
            dbContext.Meals.Add(meal);
            await dbContext.SaveChangesAsync();
            var controller = new MealController(dbContext);

            var result = await controller.DeleteMeal(meal.IdMeal);

            Assert.IsType<OkObjectResult>(result);
            var left = await dbContext.Meals.ToListAsync();
            Assert.Empty(left);
        }

        [Fact]
        public async Task EditMeal_DeberiaDevolverNotFound_SiNoExiste()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new MealController(dbContext);
            var updateDto = new MealDto { IdMeal = 99, MealName = "Error" };

            var result = await controller.EditMeal(updateDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Comida no existe en la base de datos.", notFoundResult.Value);
        }

        [Fact]
        public async Task EditMeal_DeberiaActualizar_SiExiste()
        {
            var dbContext = await GetDatabaseContext();
            var meal = new Meal { MealName = "Original" };
            dbContext.Meals.Add(meal);
            await dbContext.SaveChangesAsync();
            var controller = new MealController(dbContext);
            var updateDto = new MealDto { IdMeal = meal.IdMeal, MealName = "Editado" };

            var result = await controller.EditMeal(updateDto);

            Assert.IsType<OkResult>(result);
            var updated = await dbContext.Meals.FindAsync(meal.IdMeal);
            Assert.Equal("Editado", updated.MealName);
        }
    }
}