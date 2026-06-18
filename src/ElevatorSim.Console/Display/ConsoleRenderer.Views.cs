using ElevatorSim.Domain.Enums;
using ElevatorSim.Domain.Interfaces;

namespace ElevatorSim.Console.Display;

internal sealed partial class ConsoleRenderer
{
    private void RenderBuildingView(IReadOnlyList<IElevator> statuses)
    {
        System.Console.SetCursorPosition(0, _buildingLabelRow);
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.Write(" BUILDING  ");
        foreach (IElevator elevator in statuses)
        {
            System.Console.Write($"  E{elevator.Id} ");
        }

        System.Console.ResetColor();

        for (int floor = _building.TotalFloors; floor >= 1; floor--)
        {
            int row = _buildingFirstFloorRow + (_building.TotalFloors - floor);
            System.Console.SetCursorPosition(0, row);
            System.Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.Write($" F{floor,-3} ");

            foreach (IElevator elevator in statuses)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkGray;
                System.Console.Write("│");

                if (elevator.CurrentFloor == floor)
                {
                    System.Console.ForegroundColor = GetStatusColor(elevator.Status);
                    string cab = elevator.Direction switch
                    {
                        ElevatorDirection.Up => " ▲ ",
                        ElevatorDirection.Down => " ▼ ",
                        _ => $"E{elevator.Id} "
                    };
                    System.Console.Write(cab);
                }
                else
                {
                    System.Console.Write("   ");
                }
            }

            System.Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.Write("│");
            System.Console.ResetColor();
        }

        System.Console.SetCursorPosition(0, _buildingFooterRow);
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.Write("       ");
        for (int elevatorColumn = 0; elevatorColumn < statuses.Count; elevatorColumn++)
        {
            System.Console.Write("┴───");
        }

        System.Console.Write("┘");
        System.Console.ResetColor();
    }

    private void RenderStatusTable(IReadOnlyList<IElevator> statuses)
    {
        int row = _statusTableDataStartRow;
        foreach (IElevator elevator in statuses)
        {
            System.Console.SetCursorPosition(0, row);
            System.Console.ForegroundColor = GetStatusColor(elevator.Status);
            System.Console.Write(FormatElevatorRow(elevator));
            System.Console.ResetColor();
            row++;
        }
    }

    private static string FormatElevatorRow(IElevator elevator)
    {
        string directionSymbol = elevator.Direction switch
        {
            ElevatorDirection.Up => "▲",
            ElevatorDirection.Down => "▼",
            ElevatorDirection.Stationary => "●",
            _  => "?"
        };

        return $"║ {elevator.Id,-2} ║ {elevator.CurrentFloor,4}   ║ {directionSymbol,9} ║ {elevator.Status,-9} ║ {elevator.CurrentPassengerCount,3} / {elevator.MaximumPassengerCapacity,-3} ║ {elevator.Type,-9} ║";
    }

    private static ConsoleColor GetStatusColor(ElevatorStatus status) =>
        status switch
        {
            ElevatorStatus.Idle => ConsoleColor.Green,
            ElevatorStatus.Moving => ConsoleColor.Yellow,
            ElevatorStatus.DoorsOpen => ConsoleColor.Cyan,
            ElevatorStatus.AtCapacity => ConsoleColor.Red,
            _  => ConsoleColor.White
        };
    private void DrawStaticChrome()
    {
        System.Console.Clear();

        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║                  BUILDING ELEVATOR MONITOR                   ║");
        System.Console.ResetColor();

        for (int reservedBuildingRow = 0; reservedBuildingRow < _building.TotalFloors + 3; reservedBuildingRow++)
        {
            System.Console.WriteLine();
        }

        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine("╔════╦════════╦═══════════╦═══════════╦═════════════╦══════════╗");
        System.Console.WriteLine("║ #  ║ Floor  ║ Direction ║ Status    ║ Passengers  ║ Type     ║");
        System.Console.WriteLine("╠════╬════════╬═══════════╬═══════════╬═════════════╬══════════╣");

        for (int reservedElevatorRow = 0; reservedElevatorRow < _building.TotalElevators; reservedElevatorRow++)
        {
            System.Console.WriteLine("║".PadRight(64));
        }

        System.Console.WriteLine("╚════╩════════╩═══════════╩═══════════╩═════════════╩══════════╝");

        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.WriteLine();
        System.Console.WriteLine("  Commands:  call <floor> <passengers>  |  status (s)  |  quit (q)");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }
}