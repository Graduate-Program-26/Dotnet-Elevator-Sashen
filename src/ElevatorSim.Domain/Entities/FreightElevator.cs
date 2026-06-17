using ElevatorSim.Domain.Enums;
using ElevatorSim.Domain.Interfaces;

namespace ElevatorSim.Domain.Entities;

public sealed class FreightElevator : ElevatorBase
{
    private const int FreightPersonEquivalentCapacity = 2;
    private const int FreightTransitDelayMilliseconds = 1400;
    public int MaxLoadUnits { get; }
    public int CurrentLoadUnits { get; private set; }
    protected override int FloorTransitDelayMilliseconds => FreightTransitDelayMilliseconds;
    public override ElevatorType Type => ElevatorType.Freight;
    public override int MaximumPassengerCapacity => FreightPersonEquivalentCapacity;
    public FreightElevator(int id, int initialFloor, int maxLoadUnits) : base(id, initialFloor)
    {
        if (maxLoadUnits < 1)
            throw new ArgumentOutOfRangeException(nameof(maxLoadUnits), "Freight elevator must have a positive load capacity.");

        MaxLoadUnits = maxLoadUnits;
    }

    public void LoadCargo(int loadUnits)
    {
        if (CurrentLoadUnits + loadUnits > MaxLoadUnits)
            throw new InvalidOperationException($"Cannot load {loadUnits} units. Freight elevator capacity is {MaxLoadUnits}.");

        CurrentLoadUnits += loadUnits;
    }

    public void UnloadAllCargo() => CurrentLoadUnits = 0;

}