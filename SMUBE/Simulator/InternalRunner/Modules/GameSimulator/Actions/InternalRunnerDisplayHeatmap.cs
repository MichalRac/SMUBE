using System;
using System.Collections.Generic;
using SMUBE_Utils.Simulator.Utils.MapPrinter;
using SMUBE.BattleState.Heatmap;
using SMUBE.BattleState.Heatmap.CommandBased;
using SMUBE.DataStructures.Utils;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerDisplayHeatmap : InternalRunnerAction
    {
        public InternalRunnerDisplayHeatmap(BattleCoreSimulationWrapper coreWrapper) : base(coreWrapper)
        {
        }

        public override void OnPicked()
        {
            var activeUnitCoords = CoreWrapper.Core.currentStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates;
            var gridMapGenericDisplayData = new List<GridMapGenericDisplayData>()
            {
                new GridMapGenericDisplayData()
                {
                    Coordinates = activeUnitCoords,
                    Label = "(active)",
                    Color = ConsoleColor.Yellow
                }
            };

            var stateModel = CoreWrapper.Core.currentStateModel;
            var activeUnitTeamId = stateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId;
            
            //var heatmap = new CountReachingUnitsOfTeamIdHeatmap(activeUnitTeamId, stateModel);
            //var heatmap = new DistanceToUnitOfTeamIdHeatmap(activeUnitTeamId, false, stateModel, stateModel.ActiveUnit.UnitData.UnitIdentifier);
            var heatmap = new TeleportTargetScoreHeatmap(stateModel);
            
            heatmap.ProcessHeatmap(stateModel);

            for (int x = 0; x < heatmap.Heatmap.Count; x++)
            {
                for (int y = 0; y < heatmap.Heatmap[x].Count; y++)
                {
                    var color = heatmap.GetDebugConsoleColor(heatmap.Heatmap[x][y]);
                    var label = heatmap.GetDebugMessage(heatmap.Heatmap[x][y]);
                    
                    gridMapGenericDisplayData.Add(new GridMapGenericDisplayData()
                    {
                        Coordinates = new SMUBEVector2<int>(x, y),
                        Label = label,
                        Color = color
                    });
                }
            }
            GridMapPrinter.DefaultGridPrinter(CoreWrapper.Core.currentStateModel.BattleSceneState, gridMapGenericDisplayData).PrintMap();
        }
    }
}