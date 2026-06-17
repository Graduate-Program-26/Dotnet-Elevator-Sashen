using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.ValueObjects;

namespace ElevatorSim.Application.Interfaces;
public interface IElevatorController
{
    Task<IElevator?> DispatchElevatorAsync(ElevatorRequest request, CancellationToken cancellationToken);
    IReadOnlyList<IElevator> GetAllElevatorStatuses();
}