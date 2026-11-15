using Microsoft.EntityFrameworkCore;
namespace RentBuild.Platform.Shared.Infrastructure.Configuration;
/// <summary>Each bounded context supplies its own registration and EF model.</summary>
public interface IPlatformModule
{
    string Name { get; }
    void Register(IServiceCollection services, IConfiguration configuration);
    void ConfigureModel(ModelBuilder model);
}
public sealed class PlatformModuleCatalog
{
    public IReadOnlyList<IPlatformModule> Modules { get; }
    public PlatformModuleCatalog()
    {
        Modules = typeof(IPlatformModule).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface && typeof(IPlatformModule).IsAssignableFrom(type))
            .Select(type => (IPlatformModule)Activator.CreateInstance(type)!)
            .OrderBy(module => module.Name).ToArray();
    }
}
