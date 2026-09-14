namespace AircraftMaintenanceOperations.Application.Features.Aircraft.Queries.GetAircraft;

public record GetAircraftQueryHandler(IAircraftMaintenanceDbContext DbContext) : IQueryHandler<GetAircraftQuery, GetAircraftResult>
{
    public async Task<GetAircraftResult> Handle(GetAircraftQuery query, CancellationToken cancellationToken)
    {
        var aircrafts = await DbContext.Aircrafts
            .AsNoTracking()
            .OrderBy(a => a.TailNumber)
            .ToListAsync(cancellationToken);

        return new GetAircraftResult(aircrafts.Select(a => new AircraftDto(
            a.TailNumber,
            a.Manufacturer,
            a.Model,
            a.SerialNumber,
            a.Year)));
    }
}
