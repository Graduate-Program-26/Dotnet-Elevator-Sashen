using ElevatorSim.Domain.Enums;

namespace ElevatorSim.Domain.Interfaces;

public sealed class PassenngerElevator : ElevatorBase
{
    public override ElevatorType Type => ElevatorType.Passenger;
    public override int MaximumPassenngerCapacity { get; }
    public PassenngerElevator(int id, int initialFloor, int maximumPassengerCapacity) : base(id, initialFloor)
    {
        if (maximumPassengerCapacity < 1)
            throw new ArgumentException(nameof(maximumPassengerCapacity), "Passenger elevator must hold at least one person.");

        MaximumPassenngerCapacity = maximumPassengerCapacity;
    }

}