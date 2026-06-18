using ElevatorSim.Domain.ValueObjects;
namespace ElevatorSim.Console.Input;

internal static class BuildingConfigurationPrompt
{
    private const int _minFloors = 2;
    private const int _maxFloors = 8;
    private const int _minElevators = 1;
    private const int _maxElevators = 10;
    private const int _minCapacity = 1;
    private const int _maxCapacity = 10;
    private const int _fixedChromeOverheadRows = 13;

    private const int _scrollingAreaMarginRows = 10;

    private const int _fallbackWindowHeight = 40;

    internal static async Task<BuildingConfiguration> CollectAsync()
    {
        System.Console.Clear();
        PrintHeader();

        int maxCombinedFloorsAndElevators = GetMaxCombinedFloorsAndElevators();

        int totalFloors;
        int totalElevators;
        while (true)
        {
            totalFloors = PromptForInteger(
                prompt: $"Number of floors ({_minFloors} - {_maxFloors}): ",
                minimumValue: _minFloors,
                maximumValue: _maxFloors);

            totalElevators = PromptForInteger(
                prompt: $"Number of elevators ({_minElevators} - {_maxElevators}): ",
                minimumValue: _minElevators,
                maximumValue: _maxElevators);

            if (totalFloors + totalElevators <= maxCombinedFloorsAndElevators)
            {
                break;
            }

            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(
                $"Floors + elevators ({totalFloors + totalElevators}) won't fit in this terminal window. " +
                $"Choose a combined total of {maxCombinedFloorsAndElevators} or fewer.");
            System.Console.ResetColor();
        }

        int passengerCapacity = PromptForInteger(
            prompt: $"Passenger capacity per elevator ({_minCapacity} - {_maxCapacity}): ",
            minimumValue: _minCapacity,
            maximumValue: _maxCapacity);

        await Task.CompletedTask;
        return new BuildingConfiguration(totalFloors, totalElevators, passengerCapacity);
    }

    private static int GetMaxCombinedFloorsAndElevators()
    {
        int windowHeight = System.Console.WindowHeight > 0 ? System.Console.WindowHeight : _fallbackWindowHeight;
        int budget = windowHeight - _fixedChromeOverheadRows - _scrollingAreaMarginRows;
        return Math.Max(_minFloors + _minElevators, budget);
    }

    private static int PromptForInteger(string prompt, int minimumValue, int maximumValue)
    {
        while (true)
        {
            System.Console.Write(prompt);
            string? userInput = System.Console.ReadLine();

            if (int.TryParse(userInput, out int parsedValue) &&
                parsedValue >= minimumValue &&
                parsedValue <= maximumValue)
            {
                return parsedValue;
            }

            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"Please enter a whole number between {minimumValue} and {maximumValue}.");
            System.Console.ResetColor();
        }
    }

    private static void PrintHeader()
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("╔════════════════════════════════════╗");
        System.Console.WriteLine("║              ELEVATOR              ║");
        System.Console.WriteLine("╚════════════════════════════════════╝");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }
}