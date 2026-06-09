using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Controllers;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using SoulFoodAiBack.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UserControllerTests
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
        public async Task GetAllUsers_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            var controller = new UserController(db, null!);
            var result = await controller.GetAllUsers();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddUser_DeberiaDevolverBadRequest_SiEmailExiste()
        {
            var db = await GetDatabaseContext();
            db.Users.Add(new User { IdUser = 1, UserName = "A", Email = "test@test.com", PasswordHash = "X" });
            await db.SaveChangesAsync();

            var controller = new UserController(db, null!);
            var dto = new CreateUserDto { UserName = "B", Email = "test@test.com", PasswordHash = "123" };

            var result = await controller.AddUser(dto);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_DeberiaDevolverOk()
        {
            var db = await GetDatabaseContext();
            db.Users.Add(new User { IdUser = 1, UserName = "A", Email = "test@test.com", PasswordHash = "X" });
            await db.SaveChangesAsync();

            var controller = new UserController(db, null!);
            var result = await controller.DeleteUser(1);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}