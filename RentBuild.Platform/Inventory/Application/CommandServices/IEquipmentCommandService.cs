using RentBuild.Platform.Inventory.Domain.Model.Commands;
using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Application.Model;
namespace RentBuild.Platform.Inventory.Application.CommandServices;

public interface IEquipmentCommandService
{
    Task<Result<Equipment>> Handle(CreateEquipmentCommand c); Task<Result<Equipment>> Handle(UpdateEquipmentCommand c); Task<Result<Equipment>> Handle(ReserveEquipmentCommand c); Task<Result<Equipment>> Handle(RestrictEquipmentCommand c); Task<Result<bool>> Handle(DeleteEquipmentCommand c);
}
