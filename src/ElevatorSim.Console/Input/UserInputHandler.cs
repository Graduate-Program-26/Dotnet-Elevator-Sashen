using ElevatorSim.Application.Interfaces;
using ElevatorSim.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace ElevatorSim.Console.Input;

internal sealed class UserInputHandler(
    IElevatorController elevatorController,
    IValidator<ElevatorRequest> requestValidator,
    ILogger<UserInputHandler> logger,
    CancellationTokenSource shutdownTokenSource)
{
    private readonly IElevatorController _elevatorController = elevatorController;
    private readonly IValidator<ElevatorRequest> _requestValidator = requestValidator;
    private readonly ILogger<UserInputHandler> _logger = logger;
    private readonly CancellationTokenSource _shutdownTokenSource = shutdownTokenSource;

    internal async Task RunInputLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            System.Console.Write("\n> ");
            string? rawInput = await Task.Run(System.Console.ReadLine, cancellationToken);

            if (rawInput is null)
            {
                continue;
            }

            await HandleCommandAsync(rawInput.Trim(), cancellationToken);
        }
    }

    private async Task HandleCommandAsync(string rawInput, CancellationToken cancellationToken)
    {
        string[] commandParts = rawInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (commandParts.Length == 0)
        {
            return;
        }

        switch (commandParts[0].ToLowerInvariant())
        {
            case "call" when commandParts.Length == 3:
                await HandleCallCommandAsync(commandParts[1], commandParts[2], cancellationToken);
                break;

            case "status":
            case "s":
                PrintDetailedStatus();
                break;

            case "quit":
            case "q":
                System.Console.WriteLine("Shutting down simulation...");
                _shutdownTokenSource.Cancel();
                break;

            default:
                PrintInvalidCommand();
                break;
        }
    }

    private async Task HandleCallCommandAsync(
        string floorInput,
        string passengerCountInput,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(floorInput, out int floor) ||
            !int.TryParse(passengerCountInput, out int passengerCount))
        {
            PrintError("Usage: call <floor> <passengers> — both must be whole numbers.");
            return;
        }

        ElevatorRequest request = ElevatorRequest.Create(floor, passengerCount);
        ValidationResult validationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            foreach (ValidationFailure failure in validationResult.Errors)
            {
                PrintError(failure.ErrorMessage);
            }

            return;
        }

        await _elevatorController.DispatchElevatorAsync(request, cancellationToken);
    }

    private static void PrintDetailedStatus() =>
        System.Console.WriteLine("(Status table updates automatically above.)");

    private static void PrintError(string errorMessage)
    {
        System.Console.ForegroundColor = ConsoleColor.Red;
        System.Console.WriteLine($" {errorMessage}");
        System.Console.ResetColor();
    }

    private static void PrintInvalidCommand() =>
        PrintError("Unknown command. Type 'call <floor> <passengers>', 'status', or 'quit'.");
}