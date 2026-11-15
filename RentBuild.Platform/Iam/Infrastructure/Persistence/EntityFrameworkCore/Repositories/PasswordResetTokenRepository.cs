using RentBuild.Platform.Iam.Domain.Model.Aggregates;
using RentBuild.Platform.Iam.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
namespace RentBuild.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class PasswordResetTokenRepository(AppDbContext db) : BaseRepository<PasswordResetToken>(db), IPasswordResetTokenRepository;