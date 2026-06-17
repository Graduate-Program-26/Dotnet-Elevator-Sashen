using ElevatorSim.Application.Interfaces;
using ElevatorSim.Domain.Interfaces;

namespace ElevatorSim.Application.Strategies;

public sealed class NearestAvailableDispatchStrategy : IDispatchStrategy
{
    public IElevator? SelectElevator(
        IReadOnlyList<IElevator> availableElevators,
        int requestedFloor
    )
    {
        if (availableElevators.Count == 0)
            return null;

        return availableElevators
            .Where(elevator => elevator.IsAvailable)
            .OrderBy(elevator => Math.Abs(elevator.CurrentFloor - requestedFloor))
            .ThenBy(elevator => elevator.CurrentPassengerCount)
            .FirstOrDefault();
    }
}