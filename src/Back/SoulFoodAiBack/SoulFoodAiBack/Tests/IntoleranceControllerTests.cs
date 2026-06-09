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
    public class IntoleranceControllerTests
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
        public async Task GetAllIntolerances_DeberiaDevolverOk()
        {
            var dbContext = await GetDatabaseContext();
            dbContext.Intolerances.Add(new Intolerance { IntoleranceName = "Gluten" });
            await dbContext.SaveChangesAsync();
            var controller = new IntoleranceController(dbContext);

            var result = await controller.GetAllIntolerances();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var intolerances = Assert.IsType<List<IntoleranceDto>>(okResult.Value);
            Assert.Single(intolerances);
            Assert.Equal("Gluten", intolerances[0].IntoleranceName);
        }

        [Fact]
        public async Task AddIntolerance_DeberiaDevolverOk_YGuardar()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new IntoleranceController(dbContext);
            var newDto = new CreateIntoleranceDto { IntoleranceName = "Lactosa" };

            var result = await controller.AddIntolerance(newDto);

            Assert.IsType<OkResult>(result);
            var inDb = await dbContext.Intolerances.FirstOrDefaultAsync(i => i.IntoleranceName == "Lactosa");
            Assert.NotNull(inDb);
        }

        [Fact]
        public async Task DeleteIntolerance_DeberiaDevolverNotFound_SiNoExiste()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new IntoleranceController(dbContext);

            var result = await controller.DeleteIntolerance(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Esa intolerancia no existe.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteIntolerance_DeberiaBorrar_SiExiste()
        {
            var dbContext = await GetDatabaseContext();
            var intolerance = new Intolerance { IntoleranceName = "Borrar" };
            dbContext.Intolerances.Add(intolerance);
            await dbContext.SaveChangesAsync();
            var controller = new IntoleranceController(dbContext);

            var result = await controller.DeleteIntolerance(intolerance.IdIntolerance);

            Assert.IsType<OkObjectResult>(result);
            var left = await dbContext.Intolerances.ToListAsync();
            Assert.Empty(left);
        }

        [Fact]
        public async Task EditIntolerance_DeberiaDevolverNotFound_SiNoExiste()
        {
            var dbContext = await GetDatabaseContext();
            var controller = new IntoleranceController(dbContext);
            var updateDto = new IntoleranceDto { IdIntolerance = 99, IntoleranceName = "Error" };

            var result = await controller.EditIntolerance(updateDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Intolerancia no existe en la base de datos.", notFoundResult.Value);
        }

        [Fact]
        public async Task EditIntolerance_DeberiaActualizar_SiExiste()
        {
            var dbContext = await GetDatabaseContext();
            var intolerance = new Intolerance { IntoleranceName = "Original" };
            dbContext.Intolerances.Add(intolerance);
            await dbContext.SaveChangesAsync();
            var controller = new IntoleranceController(dbContext);
            var updateDto = new IntoleranceDto { IdIntolerance = intolerance.IdIntolerance, IntoleranceName = "Editado" };

            var result = await controller.EditIntolerance(updateDto);

            Assert.IsType<OkResult>(result);
            var updated = await dbContext.Intolerances.FindAsync(intolerance.IdIntolerance);
            Assert.Equal("Editado", updated.IntoleranceName);
        }
    }
}