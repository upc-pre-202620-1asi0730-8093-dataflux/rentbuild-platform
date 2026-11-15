using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Profiles.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Profiles.Domain.Model.Aggregates;

public sealed class CompanyProfile : AggregateRoot
{
    public int UserId
    {
        get; private set;
    }
    public string FirstName { get; private set; } = ""; public string LastName { get; private set; } = "";
    public string ContactEmail { get; private set; } = ""; public string PhoneNumber { get; private set; } = ""; public string CompanyName { get; private set; } = "";
    public CompanyAddress Address { get; private set; } = new();
    private CompanyProfile()
    {
    }
    public CompanyProfile(int userId, string first, string last, string email, string company)
    {
        UserId = userId;
        Update(first, last, email, "", company, new());
        Raise("CompanyProfileCreated");
    }
    public void Update(string first, string last, string email, string phone, string company, CompanyAddress address)
    {
        FirstName = Rule.Text(first, "First name");
        LastName = Rule.Text(last, "Last name");
        ContactEmail = Rule.Text(email, "Contact email", 254);
        Rule.Require(System.Net.Mail.MailAddress.TryCreate(ContactEmail, out _), "Invalid contact email");
        PhoneNumber = phone?.Trim() ?? "";
        Rule.Require(PhoneNumber.Length <= 30, "Phone number too long");
        CompanyName = Rule.Text(company, "Company name");
        Address = address.Validate();
    }
}
