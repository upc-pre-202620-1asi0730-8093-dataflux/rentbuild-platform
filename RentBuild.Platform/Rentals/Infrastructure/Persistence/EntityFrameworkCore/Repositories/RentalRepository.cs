using RentBuild.Platform.Rentals.Domain.Model.Aggregates;
using RentBuild.Platform.Rentals.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
namespace RentBuild.Platform.Rentals.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class RentalRepository(AppDbContext db) : BaseRepository<Rental>(db), IRentalRepository;
