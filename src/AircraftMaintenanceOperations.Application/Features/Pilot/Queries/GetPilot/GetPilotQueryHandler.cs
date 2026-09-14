namespace AircraftMaintenanceOperations.Application.Features.Pilot.Queries.GetPilot;

public record GetPilotQueryHandler(IAircraftMaintenanceDbContext dbContext) : IQueryHandler<GetPilotQuery, GetPilotResult>
{
    public async Task<GetPilotResult> Handle(GetPilotQuery query, CancellationToken cancellationToken)
    {
        var pilots = await dbContext.Pilots
            .AsNoTracking()
            .OrderBy(p => p.Rank) 
            .ToListAsync(cancellationToken);

        return new GetPilotResult(pilots.Select(p => new PilotDto(
            p.EmployeeNumber,
            p.FirstName,
            p.LastName,
            p.Email,
            p.PhoneNumber,
            p.Rank,
            p.LicenseNumber)));
    }
}
