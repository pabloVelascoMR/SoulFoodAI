using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SoulFoodAiBack.Controllers;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using SoulFoodAiBack.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class AuthControllerTests
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

        private AuthService GetAuthService()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "SuperSecretKeyParaElTestingQueTengaAlMenos32CaracteresDeLargo!!!"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            return new AuthService(config);
        }

        [Fact]
        public async Task Login_DeberiaDevolverUnauthorized_SiNoExiste()
        {
            var db = await GetDatabaseContext();
            var controller = new AuthController(db, GetAuthService());
            var result = await controller.Login(new LoginDto { Email = "no@existe.com", Password = "123" });
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_DeberiaDevolverOk_SiCredencialesSonCorrectas()
        {
            var db = await GetDatabaseContext();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("1234");
            db.Users.Add(new User { Email = "test@test.com", PasswordHash = passwordHash, UserName = "Test" });
            await db.SaveChangesAsync();
            var controller = new AuthController(db, GetAuthService());

            var result = await controller.Login(new LoginDto { Email = "test@test.com", Password = "1234" });

            Assert.IsType<OkObjectResult>(result);
        }
    }
}