using ElevatorSim.Application.Interfaces;
using ElevatorSim.Domain.Entities;
using ElevatorSim.Domain.Enums;
using ElevatorSim.Domain.Interfaces;

namespace ElevatorSim.Infrastructure.Factories;
public sealed class ElevatorFactory : IElevatorFactory
{
    private const int FreightElevatorDefaultLoadUnits = 500;

    public IElevatorControl CreateElevator(
        ElevatorType elevatorType,
        int id,
        int initialFloor,
        int capacity) =>
    elevatorType switch
    {
        ElevatorType.Passenger => new PassengerElevator(id, initialFloor, capacity),
        ElevatorType.HighSpeed => new HighSpeedElevator(id, initialFloor),
        ElevatorType.Freight => new FreightElevator(id, initialFloor, FreightElevatorDefaultLoadUnits), _ => throw new ArgumentOutOfRangeException
        (
            nameof(elevatorType),
            elevatorType,
            $"Elevator type '{elevatorType}' has no registered factory implementation."
        )
    };
}