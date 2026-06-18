using ElevatorSim.Application.Interfaces;
using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.ValueObjects;

namespace ElevatorSim.Console.Display;
internal sealed partial class ConsoleRenderer
{
    private const int _displayRefreshIntervalMilliseconds = 500;
    private const int _titleRows = 2;
    private const int _bottomMarginRowsBeforeChromeReset = 2;
    private readonly IElevatorController _controller;
    private readonly BuildingConfiguration _building;
    private readonly int _buildingLabelRow;
    private readonly int _buildingFirstFloorRow;
    private readonly int _buildingFooterRow;
    private readonly int _statusTableDataStartRow;

    internal ConsoleRenderer(IElevatorController controller, BuildingConfiguration building)
    {
        _controller = controller;
        _building = building;

        _buildingLabelRow = _titleRows + 1;
        _buildingFirstFloorRow = _buildingLabelRow + 1;
        _buildingFooterRow = _buildingFirstFloorRow + building.TotalFloors;
        _statusTableDataStartRow = _buildingFooterRow + 4; 

        DrawStaticChrome();
    }

    internal async Task RunDisplayLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (System.Console.WindowHeight > 0 &&
                    System.Console.CursorTop >= System.Console.WindowHeight - _bottomMarginRowsBeforeChromeReset)
                {
                    DrawStaticChrome();
                }

                int cursorRowBeforeRedraw = System.Console.CursorTop;
                int cursorColumnBeforeRedraw = System.Console.CursorLeft;

                IReadOnlyList<IElevator> statuses = _controller.GetAllElevatorStatuses();
                RenderBuildingView(statuses);
                RenderStatusTable(statuses);

                System.Console.SetCursorPosition(cursorColumnBeforeRedraw, cursorRowBeforeRedraw);
                await Task.Delay(_displayRefreshIntervalMilliseconds, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
