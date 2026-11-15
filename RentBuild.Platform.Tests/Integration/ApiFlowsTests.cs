using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Xunit;
namespace RentBuild.Platform.Tests.Integration;

public sealed class ApiFlowsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    async Task<JsonObject> Send(HttpClient c, HttpMethod method, string path, object? data = null, int status = 200)
    {
        using var req = new HttpRequestMessage(method, "/api/v1/" + path);
        if (data is not null)
            req.Content = JsonContent.Create(data);
        using var response = await c.SendAsync(req);
        var text = await response.Content.ReadAsStringAsync();
        Assert.True((int)response.StatusCode == status, $"Expected {status}, received {(int)response.StatusCode}: {text}");
        return string.IsNullOrEmpty(text) ? new() : JsonNode.Parse(text)!.AsObject();
    }
    async Task<JsonArray> List(HttpClient c, string path)
    {
        using var r = await c.GetAsync("/api/v1/" + path);
        Assert.True(r.IsSuccessStatusCode, await r.Content.ReadAsStringAsync());
        return (JsonNode.Parse(await r.Content.ReadAsStringAsync())!).AsArray();
    }
    async Task<(HttpClient Client, int Id, string Email)> Account(string role)
    {
        var c = factory.CreateClient();
        var email = Guid.NewGuid() + "@rentbuild.test";
        var user = await Send(c, HttpMethod.Post, "authentication/sign-up", new
        {
            firstName = "Test",
            lastName = "Company",
            email,
            password = "RentBuild123!",
            companyName = "QA Company",
            role
        }, 201);
        var auth = await Send(c, HttpMethod.Post, "authentication/sign-in", new
        {
            email,
            password = "RentBuild123!"
        });
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth["token"]!.GetValue<string>());
        return (c, user["id"]!.GetValue<int>(), email);
    }
    async Task<JsonObject> Subscribe(HttpClient c, int id, int plan = 3) => await Send(c, HttpMethod.Post, "user-subscriptions", new { userId = id, planId = plan, startDate = "2090-01-01T00:00:00Z", endDate = "2090-02-01T00:00:00Z", status = "ACTIVE", autoRenew = true }, 201);
    async Task<JsonObject> Equipment(HttpClient c, int id) => await Send(c, HttpMethod.Post, "equipment", new { userId = id, code = "QA-" + Guid.NewGuid(), name = "Excavadora QA", description = "Integration test", categoryId = 1, location = "Lima", dailyRate = 100, weeklyRate = 500, currency = "PEN", status = "AVAILABLE", availabilityBlocks = Array.Empty<object>() }, 201);
    static int Id(JsonObject r) => r["id"]!.GetValue<int>();
    static object Period() => new { startDate = DateTimeOffset.UtcNow, endDate = DateTimeOffset.UtcNow.AddDays(1) };
    async Task<JsonObject> Request((HttpClient Client, int Id, string Email) builder, int owner, int equipment) => await Send(builder.Client, HttpMethod.Post, "rental-requests", new { equipmentId = equipment, constructionUserId = builder.Id, rentalCompanyUserId = owner, startDate = DateTimeOffset.UtcNow, endDate = DateTimeOffset.UtcNow.AddDays(1), status = "PENDING" }, 201);

    [Fact]
    public async Task AuthenticationAndProfileAreAtomicAndPrivate()
    {
        var owner = await Account("rental_company");
        var profiles = await List(owner.Client, "profiles?userId=" + owner.Id);
        Assert.Single(profiles);
        Assert.Equal(owner.Email, profiles[0]!["contactEmail"]!.GetValue<string>());
        var other = await Account("construction_company");
        var directory = await List(other.Client, "profiles?userId=" + owner.Id);
        Assert.Equal("", directory[0]!["contactEmail"]!.GetValue<string>());
        Assert.Null(directory[0]!["address"]);
        var profile = profiles[0]!.AsObject();
        profile["companyName"] = "Updated company";
        await Send(owner.Client, HttpMethod.Put, "profiles/" + Id(profile), profile);
        await Send(other.Client, HttpMethod.Put, "profiles/" + Id(profile), profile, 403);
        var email = Guid.NewGuid() + "@rentbuild.test";
        await Send(factory.CreateClient(), HttpMethod.Post, "authentication/sign-up", new
        {
            firstName = "Test",
            lastName = "User",
            email,
            password = "RentBuild123!",
            companyName = new string('x', 201),
            role = "rental_company"
        }, 400);
        await Send(factory.CreateClient(), HttpMethod.Post, "authentication/sign-in", new
        {
            email,
            password = "RentBuild123!"
        }, 401);
    }
    [Fact]
    public async Task RentalLifecycleIsAtomicAndReleasesReservations()
    {
        var owner = await Account("rental_company");
        var builder = await Account("construction_company");
        await Subscribe(owner.Client, owner.Id);
        var e = await Equipment(owner.Client, owner.Id);
        int equipment = Id(e);
        var request = await Request(builder, owner.Id, equipment);
        int requestId = Id(request);
        await Send(builder.Client, HttpMethod.Patch, $"rental-requests/{requestId}/period", Period());
        await Send(owner.Client, HttpMethod.Patch, $"rental-requests/{requestId}/period", Period(), 403);
        request["status"] = "APPROVED";
        await Send(owner.Client, HttpMethod.Put, "rental-requests/" + requestId, request);
        var rentals = await List(owner.Client, "rentals");
        Assert.Single(rentals);
        int rentalId = rentals[0]!["id"]!.GetValue<int>();
        await Send(owner.Client, HttpMethod.Put, "rental-requests/" + requestId, request, 409);
        e = await Send(owner.Client, HttpMethod.Get, "equipment/" + equipment);
        Assert.Single(e["availabilityBlocks"]!.AsArray());
        await Send(owner.Client, HttpMethod.Post, "deliveries", new
        {
            rentalId,
            deliveredAt = DateTimeOffset.UtcNow,
            notes = "Delivered"
        }, 201);
        await Send(owner.Client, HttpMethod.Post, "equipment-returns", new
        {
            rentalId,
            returnedAt = DateTimeOffset.UtcNow,
            notes = "Inspection needed",
            maintenanceRequired = true
        }, 201);
        e = await Send(owner.Client, HttpMethod.Get, "equipment/" + equipment);
        Assert.Equal("MAINTENANCE", e["status"]!.GetValue<string>());
        Assert.Empty(e["availabilityBlocks"]!.AsArray());
        await Send(owner.Client, HttpMethod.Post, $"incidents/equipment/{equipment}/reactivate", new
        {
            inspectionConfirmed = true
        }, 409);
        var works = await List(owner.Client, "maintenances");
        Assert.Single(works);
        int workId = works[0]!["id"]!.GetValue<int>();
        await Send(owner.Client, HttpMethod.Patch, "maintenances/" + workId, new
        {
            status = "COMPLETED",
            technician = "QA Technician",
            acknowledge = true,
            notes = "Checked"
        });
        Assert.Single(await List(owner.Client, "maintenances"));
        var incident = await Send(owner.Client, HttpMethod.Post, "incidents", new
        {
            equipmentId = equipment,
            description = "Fault",
            blocksRental = true
        }, 201);
        await Send(owner.Client, HttpMethod.Post, $"incidents/equipment/{equipment}/reactivate", new
        {
            inspectionConfirmed = true
        }, 409);
        await Send(owner.Client, HttpMethod.Patch, "incidents/" + Id(incident), new
        {
            status = "RESOLVED"
        });
        await Send(owner.Client, HttpMethod.Post, $"incidents/equipment/{equipment}/reactivate", new
        {
            inspectionConfirmed = true
        }, 204);
        var history = await Send(owner.Client, HttpMethod.Get, $"maintenances/equipment/{equipment}/history");
        Assert.Equal(3, history["entries"]!.AsArray().Select(x => x!["kind"]!.GetValue<string>()).Distinct().Count());
        await Send(builder.Client, HttpMethod.Get, $"maintenances/equipment/{equipment}/history", null, 403);
    }
    [Fact]
    public async Task ConcurrentApprovalsCannotDoubleBookEquipment()
    {
        var owner = await Account("rental_company");
        await Subscribe(owner.Client, owner.Id);
        var e = await Equipment(owner.Client, owner.Id);
        var a = await Account("construction_company");
        var b = await Account("construction_company");
        var first = await Request(a, owner.Id, Id(e));
        var second = await Request(b, owner.Id, Id(e));
        first["status"] = "APPROVED";
        second["status"] = "APPROVED";
        var replies = await Task.WhenAll(owner.Client.PutAsJsonAsync("/api/v1/rental-requests/" + Id(first), first), owner.Client.PutAsJsonAsync("/api/v1/rental-requests/" + Id(second), second));
        Assert.Equal(new[] { 200, 409 }, replies.Select(r => (int)r.StatusCode).Order().ToArray());
        Assert.Single(await List(owner.Client, "rentals"));
        var saved = await Send(owner.Client, HttpMethod.Get, "equipment/" + Id(e));
        Assert.Single(saved["availabilityBlocks"]!.AsArray());
    }
    [Fact]
    public async Task SubscriptionDatesComeFromServerAndCancelRevokesAccess()
    {
        var owner = await Account("rental_company");
        var sub = await Subscribe(owner.Client, owner.Id);
        Assert.True(DateTimeOffset.Parse(sub["startDate"]!.GetValue<string>()) < DateTimeOffset.UtcNow.AddMinutes(1));
        var before = sub["endDate"]!.GetValue<string>();
        sub["planId"] = 2;
        var changed = await Send(owner.Client, HttpMethod.Put, "user-subscriptions/" + Id(sub), sub);
        Assert.Equal(before, changed["endDate"]!.GetValue<string>());
        await Send(owner.Client, HttpMethod.Patch, $"user-subscriptions/{Id(sub)}/cancel", new
        {
        });
        await Send(owner.Client, HttpMethod.Post, "equipment", new
        {
            userId = owner.Id,
            code = "Denied",
            name = "Machine",
            description = "Test",
            categoryId = 1,
            location = "Lima",
            dailyRate = 100,
            weeklyRate = 500
        }, 403);
    }
    [Fact]
    public async Task RecoveryIsSingleUseAndRevokesExistingTokens()
    {
        var owner = await Account("rental_company");
        var anonymous = factory.CreateClient();
        await Send(anonymous, HttpMethod.Post, "authentication/password-recovery", new
        {
            email = owner.Email
        }, 202);
        var token = factory.Mail.Tokens[owner.Email];
        await Send(anonymous, HttpMethod.Post, "authentication/reset-password", new
        {
            token,
            password = "NewRentBuild123!"
        });
        await Send(anonymous, HttpMethod.Post, "authentication/reset-password", new
        {
            token,
            password = "OtherPassword123!"
        }, 400);
        using var expired = await owner.Client.GetAsync("/api/v1/authentication/me");
        Assert.Equal(HttpStatusCode.Unauthorized, expired.StatusCode);
        await Send(anonymous, HttpMethod.Post, "authentication/sign-in", new
        {
            email = owner.Email,
            password = "NewRentBuild123!"
        });
    }
    [Fact]
    public async Task AnonymousAccessIsRejectedAndDocumentationLoads()
    {
        using var c = factory.CreateClient();
        using var denied = await c.GetAsync("/api/v1/equipment");
        Assert.Equal(HttpStatusCode.Unauthorized, denied.StatusCode);
        using var open = await c.GetAsync("/openapi/v1.json");
        Assert.True(open.IsSuccessStatusCode, await open.Content.ReadAsStringAsync());
        var doc = JsonNode.Parse(await open.Content.ReadAsStringAsync())!;
        Assert.True(doc["paths"]!.AsObject().Count >= 30);
    }
}
