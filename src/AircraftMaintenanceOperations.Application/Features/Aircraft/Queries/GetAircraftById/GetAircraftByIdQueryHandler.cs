namespace AircraftMaintenanceOperations.Application.Features.Aircraft.Queries.GetAircraftById;

public class GetAircraftByIdQueryHandler(IAircraftMaintenanceDbContext dbContext) : IQueryHandler<GetAircraftByIdQuery, GetAircraftByIdQueryResult>
{
    public async Task<GetAircraftByIdQueryResult> Handle(GetAircraftByIdQuery query, CancellationToken cancellationToken)
    {
        var aircrafts = await dbContext.Aircrafts
            .AsNoTracking()
            .Where(a => a.Id == query.AircraftId)
            .OrderBy(a => a.TailNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetAircraftByIdQueryResult(aircrafts == null ? null : new AircraftDto(
            aircrafts.TailNumber,
            aircrafts.Manufacturer,
            aircrafts.Model, 
            aircrafts.SerialNumber,
            aircrafts.Year));
    }
}
