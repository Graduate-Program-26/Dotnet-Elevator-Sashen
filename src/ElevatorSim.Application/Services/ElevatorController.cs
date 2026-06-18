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

    private const int _noElevatorAvailableRetryDelayMilliseconds = 1000;
    private const int _deliveringPassengersDisplayDelayMilliseconds = 1500;

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

            // This model has no separate destination floor — boarding and delivery
            // are the same trip. Hold here briefly so the status table has a chance
            // to show the boarded passengers, then disembark to free the elevator's
            // capacity back up. Without disembarking, CurrentPassengerCount never
            // drops and IsAvailable stays false forever once an elevator fills up.
            await Task.Delay(_deliveringPassengersDisplayDelayMilliseconds, cancellationToken);
            await elevator.DisembarkAllPassengersAsync(cancellationToken);

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
            IElevator? dispatchedElevator = await DispatchElevatorAsync(pendingRequest, _shutdownTokenSource.Token);

            if (dispatchedElevator is null)
            {
                await Task.Delay(_noElevatorAvailableRetryDelayMilliseconds, _shutdownTokenSource.Token);
            }
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