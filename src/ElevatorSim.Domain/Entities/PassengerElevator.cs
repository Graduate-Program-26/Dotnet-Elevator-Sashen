using ElevatorSim.Domain.Enums;

namespace ElevatorSim.Domain.Entities;

public sealed class PassengerElevator : ElevatorBase
{
    public override ElevatorType Type => ElevatorType.Passenger;
    public override int MaximumPassengerCapacity { get; }

    public PassengerElevator(int id, int initialFloor, int maximumPassengerCapacity) : base(id, initialFloor)
    {
        if (maximumPassengerCapacity < 1)
            throw new ArgumentOutOfRangeException(nameof(maximumPassengerCapacity), "Passenger elevator must hold at least one person.");

        MaximumPassengerCapacity = maximumPassengerCapacity;
    }
}