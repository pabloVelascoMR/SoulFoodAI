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
    public class IngredientControllerTests
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
        public async Task GetIngredients_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.Ingredients.Add(new Ingredient { Name = "Arroz", Category = "Cereal", IsDeleted = false });
            await db.SaveChangesAsync();
            var controller = new IngredientController(db);
            var result = await controller.GetIngredients("Cereal", 1);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddCustomIngredient_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            var controller = new IngredientController(db);
            var dto = new CustomIngredientDto { Name = "Avena", Category = "Cereal", UserId = 1 };
            var result = await controller.AddCustomIngredient(dto);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteCustomIngredient_DeberiaDevolverForbid_SiNoEsCreador()
        {
            var db = await GetDatabaseContext();
            db.Ingredients.Add(new Ingredient { IdIngredient = 1, Name = "Avena", Category = "Cat", CreatedByUserId = 2 });
            await db.SaveChangesAsync();
            var controller = new IngredientController(db);
            var result = await controller.DeleteCustomIngredient(1, 1);
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task AddDefaultIngredient_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            var controller = new IngredientController(db);
            var dto = new CreateIngredientDto { Name = "Lechuga", Category = "Verdura" };
            var result = await controller.AddIngredient(dto);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateImageIngredients_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.Ingredients.Add(new Ingredient { IdIngredient = 1, Name = "Tomate", Category = "Verdura" });
            await db.SaveChangesAsync();
            var controller = new IngredientController(db);
            var dto = new List<UpdateImageIngredientDto> { new UpdateImageIngredientDto { IdIngredient = 1, ImageUrl = "url" } };
            var result = await controller.UpdateIngredients(dto);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}