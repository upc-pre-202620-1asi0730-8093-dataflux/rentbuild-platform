using Xunit;
namespace RentBuild.Platform.Tests.Architecture;

public sealed class BoundedContextTests
{
    static string Root()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null && !Directory.Exists(Path.Combine(d.FullName, "RentBuild.Platform")))
            d = d.Parent;
        return Path.Combine(d!.FullName, "RentBuild.Platform");
    }
    [Fact]
    public void DomainDoesNotDependOnFrameworksOrAnotherContext()
    {
        foreach (var context in new[] { "Iam", "Profiles", "Inventory", "Rentals", "Maintenance", "Subscriptions" })
            foreach (var file in Directory.GetFiles(Path.Combine(Root(), context, "Domain"), "*.cs", SearchOption.AllDirectories))
            {
                var source = File.ReadAllText(file);
                Assert.DoesNotContain("Microsoft.EntityFrameworkCore", source);
                Assert.DoesNotContain("Microsoft.AspNetCore", source);
                foreach (var other in new[] { "Iam", "Profiles", "Inventory", "Rentals", "Maintenance", "Subscriptions" }.Where(c => c != context))
                    Assert.DoesNotContain("using RentBuild.Platform." + other + ".", source);
            }
    }
    [Fact]
    public void ApplicationHasNoControllersOrPersistenceDependencies()
    {
        foreach (var context in new[] { "Iam", "Profiles", "Inventory", "Rentals", "Maintenance", "Subscriptions" })
            foreach (var file in Directory.GetFiles(Path.Combine(Root(), context, "Application"), "*.cs", SearchOption.AllDirectories))
            {
                var source = File.ReadAllText(file);
                Assert.DoesNotContain("Microsoft.EntityFrameworkCore", source);
                Assert.DoesNotContain("Microsoft.AspNetCore", source);
                Assert.DoesNotContain("AppDbContext", source);
                Assert.DoesNotContain(".Infrastructure.", source);
            }
    }
}
