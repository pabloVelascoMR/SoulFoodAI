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
    public class RecipeControllerTests
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
            var inMemorySettings = new Dictionary<string, string> { { "Gemini:ApiKey", "TEST_KEY" } };
            return new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        }

        [Fact]
        public async Task GetRecipesForUser_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            var controller = new RecipeController(db, GetConfig());
            var result = await controller.GetRecipesForUser(1);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddRecipesForUser_DeberiaDevolverNotFound()
        {
            var db = await GetDatabaseContext();
            var controller = new RecipeController(db, GetConfig());
            var result = await controller.AddRecipesForUser(99, new AddRecipeDto { RecipeName = "Test" });
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddRecipesForUser_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.Users.Add(new User { IdUser = 1, UserName = "T", Email = "t@t.com", PasswordHash = "x" });
            await db.SaveChangesAsync();
            var controller = new RecipeController(db, GetConfig());

            var dto = new AddRecipeDto { RecipeName = "Ensalada", IdIngredients = new List<int>(), Quantity = new List<float>(), Unit = new List<string>() };
            var result = await controller.AddRecipesForUser(1, dto);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task CreateRecipeAI_DeberiaDevolverBadRequest_SiFaltanDatos()
        {
            var db = await GetDatabaseContext();
            var controller = new RecipeController(db, GetConfig());
            var result = await controller.CreateRecipeAI(1, new CreateAiRecipeDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ArchiveRecipe_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.Recipes.Add(new Recipe { IdRecipe = 1, RecipeName = "Test", IdUser = 1 });
            await db.SaveChangesAsync();
            var controller = new RecipeController(db, GetConfig());
            var result = await controller.ArchiveRecipe(1);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}