using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkAhead.Application.MonitoredAreas;
using ParkAhead.Domain.Entities;
using ParkAhead.Infrastructure.Persistence;

namespace ParkAhead.Api.Controllers;

[ApiController]
[Route("api/monitored-areas")]
public class MonitoredAreasController : ControllerBase
{
    private readonly ParkAheadDbContext _dbContext;

    public MonitoredAreasController(ParkAheadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MonitoredAreaResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var areas = await _dbContext.MonitoredAreas
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        return Ok(areas.Select(a => a.ToResponse()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MonitoredAreaResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var area = await _dbContext.MonitoredAreas
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return area is null ? NotFound() : Ok(area.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<MonitoredAreaResponse>> Create(
        CreateMonitoredAreaRequest request,
        CancellationToken cancellationToken)
    {
        var area = new MonitoredArea
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            RadiusMeters = request.RadiusMeters,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.MonitoredAreas.Add(area);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = area.Id }, area.ToResponse());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var area = await _dbContext.MonitoredAreas
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (area is null)
        {
            return NotFound();
        }

        _dbContext.MonitoredAreas.Remove(area);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
