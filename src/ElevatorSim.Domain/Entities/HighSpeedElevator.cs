using ElevatorSim.Domain.Enums;

namespace ElevatorSim.Domain.Entities;

public class HighSpeedElevator : ElevatorBase
{
    private const int HighSpeedCapacity = 6;
    protected override int FloorTransitDelayMilliseconds => 400;
    public override ElevatorType Type => ElevatorType.HighSpeed;
    public override int MaximumPassengerCapacity => HighSpeedPassengerCapacity;
    public HighSpeedElevator(int id, int initialFloor) : base(id, initialFloor) {}
}