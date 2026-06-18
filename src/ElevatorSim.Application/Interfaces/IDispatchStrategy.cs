using ElevatorSim.Domain.Interfaces;

namespace ElevatorSim.Application.Interfaces;

public interface IDispatchStrategy
{
    IElevator? SelectElevator(IReadOnlyList<IElevator> availableElevators, int requestedFloor);
}
