namespace ElevatorSim.Domain.Exceptions;
public abstract class ElevatorSimException : Exception
{
    protected ElevatorSimException(string message) : base(message) { }
    protected ElevatorSimException(string message, Exception innerException) : base(message, innerException) { }
}