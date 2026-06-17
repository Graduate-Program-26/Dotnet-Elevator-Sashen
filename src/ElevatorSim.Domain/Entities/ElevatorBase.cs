using ElevatorSim.Domain.Enums;
using ElevatorSim.Domain.Exceptions;
using ElevatorSim.Domain.Interfaces;

namespace ElevatorSim.Domain.Entities;

public abstract class ElevatorBase : IElevator, IElevatorControl
{
    private int _currentPassengerCount;
    protected virtual int FloorTransitDelayMilliseconds => 800;
    public int Id { get; }
    public int CurrentFloor { get; protected set; }
    public ElevatorDirection Direction { get; protected set; }
    public ElevatorStatus Status { get; protected set; }
    public abstract ElevatorType Type { get; }
    public abstract int MaximumPassengerCapacity { get; }

    public int CurrentPassengerCount
    {
        get => _currentPassengerCount;
        protected set => _currentPassengerCount = value;
    }

    public event EventHandler<int>? FloorReached;

    protected ElevatorBase(int id, int initialFloor)
    {
        Id = id;
        CurrentFloor = initialFloor;
        Direction = ElevatorDirection.Stationary;
        Status = ElevatorStatus.Idle;
        _currentPassengerCount = 0;
    }

    public virtual async Task MoveToFloorAsync(int targetFloor, CancellationToken cancellationToken)
    {
        if (targetFloor == CurrentFloor)
        {
            Status = ElevatorStatus.Idle;
            return;
        }

        Direction = (targetFloor - CurrentFloor) switch
        {
            > 0 => ElevatorDirection.Up,
            < 0 => ElevatorDirection.Down,
            _ => ElevatorDirection.Stationary
        };

        Status = ElevatorStatus.Moving;

        while (CurrentFloor != targetFloor)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(FloorTransitDelayMilliseconds, cancellationToken);
            CurrentFloor += Direction switch
            {
                ElevatorDirection.Up =>  1,
                ElevatorDirection.Down => -1,
                _ =>  0
            };   
            FloorReached?.Invoke(this, CurrentFloor);
        }

        Direction = ElevatorDirection.Stationary;
        Status = ElevatorStatus.Idle;
    }

    public virtual async Task BoardPassengersAsync(int passengerCount, CancellationToken cancellationToken)
    {
        int projectedTotal = _currentPassengerCount + passengerCount;
        if (projectedTotal > MaximumPassengerCapacity)
        {
            throw new CapacityExceededException(Id, MaximumPassengerCapacity, projectedTotal);
        }
        _currentPassengerCount = projectedTotal;

        if (_currentPassengerCount == MaximumPassengerCapacity)
        {
            Status = ElevatorStatus.AtCapacity;
        }
        await Task.Delay(500, cancellationToken);
        Status = ElevatorStatus.Idle;
    }

    public bool IsAvailable =>
        Status is ElevatorStatus.Idle && _currentPassengerCount < MaximumPassengerCapacity;

    public virtual async Task DisembarkAllPassengersAsync(CancellationToken cancellationToken)
    {
        _currentPassengerCount = 0;
        Status = ElevatorStatus.DoorsOpen;
        await Task.Delay(500, cancellationToken);
        Status = ElevatorStatus.Idle;
    }
}