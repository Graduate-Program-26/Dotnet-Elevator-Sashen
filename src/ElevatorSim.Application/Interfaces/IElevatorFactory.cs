using ElevatorSim.Domain.Enums;
using ElevatorSim.Domain.Interfaces;

namespace ElevatorSim.Application.Interfaces;
public interface IElevatorFactory
{
    IElevatorControl CreateElevator(ElevatorType elevatorType, int id, int initialFloor, int capacity);
}