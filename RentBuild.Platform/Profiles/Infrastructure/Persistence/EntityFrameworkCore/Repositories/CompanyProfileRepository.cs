using RentBuild.Platform.Profiles.Domain.Model.Aggregates;
using RentBuild.Platform.Profiles.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
namespace RentBuild.Platform.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class CompanyProfileRepository(AppDbContext db) : BaseRepository<CompanyProfile>(db), ICompanyProfileRepository;