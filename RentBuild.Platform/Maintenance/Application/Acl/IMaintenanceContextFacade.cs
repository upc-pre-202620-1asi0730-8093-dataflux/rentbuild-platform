namespace RentBuild.Platform.Maintenance.Application.Acl;

public interface IMaintenanceContextFacade
{
    Task<bool> HasBlockingIncident(int equipmentId); Task ScheduleReturnWork(int equipmentId, int owner);
}
