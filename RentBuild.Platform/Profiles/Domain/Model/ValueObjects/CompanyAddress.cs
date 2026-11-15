using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Profiles.Domain.Model.ValueObjects;

public sealed record CompanyAddress(string Street = "", string District = "", string City = "", string Country = "", double Latitude = 0, double Longitude = 0)
{
    public CompanyAddress Validate()
    {
        Rule.Require(double.IsFinite(Latitude) && double.IsFinite(Longitude) && Latitude is >= -90 and <= 90 && Longitude is >= -180 and <= 180, "Invalid address coordinates");
        Rule.Require(new[] { Street, District, City, Country }.All(v => v is not null && v.Length <= 250), "Invalid address fields");
        return this;
    }
}
