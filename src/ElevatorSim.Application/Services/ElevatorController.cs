using System.Threading.Channels;
using ElevatorSim.Application.Interfaces;
using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ElevatorSim.Application.Services;
public sealed class ElevatorController : IElevatorController, IAsyncDisposable
{
    private readonly IDispatchStrategy _dispatchStrategy;
    private readonly ILogger<ElevatorController> _logger;
    private readonly List<IElevatorControl> _elevators;
    private readonly Channel<ElevatorRequest> _pendingRequestChannel;
    private readonly Task _requestProcessorTask;
    private readonly CancellationTokenSource _shutdownTokenSource;

    public ElevatorController(
        IDispatchStrategy dispatchStrategy,
        ILogger<ElevatorController> logger,
        IEnumerable<IElevatorControl> elevators)
    {
        _dispatchStrategy = dispatchStrategy;
        _logger = logger;
        _elevators = elevators.ToList();
        _shutdownTokenSource = new CancellationTokenSource();
        _pendingRequestChannel = Channel.CreateUnbounded<ElevatorRequest>();
        _requestProcessorTask = Task.Run(ProcessPendingRequestsAsync);
    }

    public async Task<IElevator?> DispatchElevatorAsync(
        ElevatorRequest request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<IElevator> elevatorStatusViews = GetAllElevatorStatuses();
        IElevator? selectedElevator = _dispatchStrategy.SelectElevator(elevatorStatusViews, request.RequestedFloor);

        if (selectedElevator is null)
        {
            _logger.LogWarning("No available elevator for floor {Floor}. Request queued.", request.RequestedFloor);
            await _pendingRequestChannel.Writer.WriteAsync(request, cancellationToken);
            return null;
        }

        _logger.LogInformation(
            "Elevator {ElevatorId} dispatched to floor {Floor} for {PassengerCount} passengers.",
            selectedElevator.Id, request.RequestedFloor, request.PassengerCount);

        IElevatorControl elevatorControl = _elevators.First(elevator => elevator.Id == selectedElevator.Id);
        _ = Task.Run(
            () => ExecuteElevatorTripAsync(elevatorControl, request, cancellationToken),
            cancellationToken);

        return selectedElevator;
    }

    public IReadOnlyList<IElevator> GetAllElevatorStatuses() =>
        _elevators.OfType<IElevator>().ToList().AsReadOnly();

    private async Task ExecuteElevatorTripAsync(
        IElevatorControl elevator,
        ElevatorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await elevator.MoveToFloorAsync(request.RequestedFloor, cancellationToken);

            int passengersThisElevatorCanTake = Math.Min(
                request.PassengerCount,
                elevator.MaximumPassengerCapacity - elevator.CurrentPassengerCount);

            await elevator.BoardPassengersAsync(passengersThisElevatorCanTake, cancellationToken);

            int remainingPassengers = request.PassengerCount - passengersThisElevatorCanTake;
            if (remainingPassengers > 0)
            {
                _logger.LogInformation(
                    "Elevator {Id} at capacity. Dispatching additional elevator for {Remaining} remaining passengers.",
                    elevator.Id, remainingPassengers);

                ElevatorRequest overflowRequest = request with { PassengerCount = remainingPassengers };
                await _pendingRequestChannel.Writer.WriteAsync(overflowRequest, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Elevator trip for floor {Floor} cancelled.", request.RequestedFloor);
        }
    }

    private async Task ProcessPendingRequestsAsync()
    {
        await foreach (ElevatorRequest pendingRequest in
            _pendingRequestChannel.Reader.ReadAllAsync(_shutdownTokenSource.Token))
        {
            await DispatchElevatorAsync(pendingRequest, _shutdownTokenSource.Token);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _shutdownTokenSource.CancelAsync();
        _pendingRequestChannel.Writer.Complete();
        await _requestProcessorTask.ConfigureAwait(false);
        _shutdownTokenSource.Dispose();
    }
}