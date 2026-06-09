using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Controllers;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UserIngredientControllerTests
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
        public async Task AddFavorite_DeberiaDevolverBadRequest_SiUserInvalido()
        {
            var db = await GetDatabaseContext();
            var controller = new UserIngredientController(db);
            var result = await controller.AddFavorite(new SaveFavouriteIngredientDto { IdUser = 0 });
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddFavorite_DeberiaDevolverOk_SiEsLocal()
        {
            var db = await GetDatabaseContext();
            db.Users.Add(new User { IdUser = 1, UserName = "A", Email = "a@a.com", PasswordHash = "1" });
            db.Ingredients.Add(new Ingredient { IdIngredient = 1, Name = "Pollo", Category = "Carne" });
            await db.SaveChangesAsync();

            var controller = new UserIngredientController(db);
            var result = await controller.AddFavorite(new SaveFavouriteIngredientDto { IdUser = 1, LocalIdIngredient = 1 });
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RemoveFavorite_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.UserIngredients.Add(new UserIngredient { IdUser = 1, IdIngredient = 1 });
            await db.SaveChangesAsync();
            var controller = new UserIngredientController(db);
            var result = await controller.RemoveFavorite(1, 1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetFavorites_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.UserIngredients.Add(new UserIngredient { IdUser = 1, IdIngredient = 1 });
            await db.SaveChangesAsync();
            var controller = new UserIngredientController(db);
            var result = await controller.GetFavorites(1);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}