namespace RentBuild.Platform.Inventory.Domain.Model.Commands;

public sealed record EquipmentFields(string Code, string Name, string Description, int CategoryId, string Location, decimal DailyRate, decimal WeeklyRate, string Currency);
public sealed record CreateEquipmentCommand(int UserId, EquipmentFields Fields);
public sealed record UpdateEquipmentCommand(int Id, EquipmentFields Fields);
public sealed record ReserveEquipmentCommand(int Id, DateOnly StartDate, DateOnly EndDate);
public sealed record DeleteEquipmentCommand(int Id);
public sealed record RestrictEquipmentCommand(int Id, string Status);
