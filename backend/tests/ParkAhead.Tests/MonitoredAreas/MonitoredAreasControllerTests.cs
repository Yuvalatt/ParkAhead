using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkAhead.Api.Controllers;
using ParkAhead.Application.MonitoredAreas;
using ParkAhead.Domain.Enums;
using ParkAhead.Infrastructure.Persistence;

namespace ParkAhead.Tests.MonitoredAreas;

public class MonitoredAreasControllerTests
{
    private static MonitoredAreasController CreateController(out ParkAheadDbContext dbContext)
    {
        var options = new DbContextOptionsBuilder<ParkAheadDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        dbContext = new ParkAheadDbContext(options);
        return new MonitoredAreasController(dbContext);
    }

    private static CreateMonitoredAreaRequest HomeRequest(string name = "Home", AreaType areaType = AreaType.Home) =>
        new(name, areaType, "1 Rothschild Blvd, Tel Aviv-Yafo, Israel", 32.08, 34.78, 1500);

    [Fact]
    public async Task Create_returns_201_and_persists_the_area()
    {
        var controller = CreateController(out var dbContext);
        var request = HomeRequest();

        var result = await controller.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        var response = Assert.IsType<MonitoredAreaResponse>(created.Value);
        Assert.Equal("Home", response.Name);
        Assert.Equal(AreaType.Home, response.AreaType);
        Assert.Equal("1 Rothschild Blvd, Tel Aviv-Yafo, Israel", response.Address);

        Assert.Equal(1, await dbContext.MonitoredAreas.CountAsync());
    }

    [Theory]
    [InlineData(AreaType.Home)]
    [InlineData(AreaType.Work)]
    [InlineData(AreaType.Other)]
    public async Task Create_persists_the_chosen_area_type(AreaType areaType)
    {
        var controller = CreateController(out var dbContext);

        await controller.Create(HomeRequest(areaType: areaType), CancellationToken.None);

        var saved = await dbContext.MonitoredAreas.SingleAsync();
        Assert.Equal(areaType, saved.AreaType);
    }

    [Fact]
    public async Task GetById_returns_the_area_when_it_exists()
    {
        var controller = CreateController(out var dbContext);
        var createResult = await controller.Create(HomeRequest(), CancellationToken.None);
        var created = (MonitoredAreaResponse)((CreatedAtActionResult)createResult.Result!).Value!;

        var result = await controller.GetById(created.Id, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<MonitoredAreaResponse>(ok.Value);
        Assert.Equal(created.Id, response.Id);
    }

    [Fact]
    public async Task GetById_returns_404_when_the_area_does_not_exist()
    {
        var controller = CreateController(out _);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_returns_every_persisted_area()
    {
        var controller = CreateController(out _);
        await controller.Create(HomeRequest(), CancellationToken.None);
        await controller.Create(HomeRequest("Office", AreaType.Work), CancellationToken.None);

        var result = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var areas = Assert.IsAssignableFrom<IEnumerable<MonitoredAreaResponse>>(ok.Value);
        Assert.Equal(2, areas.Count());
    }

    [Fact]
    public async Task Delete_removes_an_existing_area_and_returns_204()
    {
        var controller = CreateController(out var dbContext);
        var createResult = await controller.Create(HomeRequest(), CancellationToken.None);
        var created = (MonitoredAreaResponse)((CreatedAtActionResult)createResult.Result!).Value!;

        var result = await controller.Delete(created.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, await dbContext.MonitoredAreas.CountAsync());
    }

    [Fact]
    public async Task Delete_returns_404_when_the_area_does_not_exist()
    {
        var controller = CreateController(out _);

        var result = await controller.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }
}
