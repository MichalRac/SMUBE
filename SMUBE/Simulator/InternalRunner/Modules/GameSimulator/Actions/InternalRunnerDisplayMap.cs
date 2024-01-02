using System.Collections.Generic;
using SMUBE_Utils.Simulator.Utils.MapPrinter;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerDisplayMap : InternalRunnerAction
    {
        public InternalRunnerDisplayMap(BattleCoreSimulationWrapper coreWrapper) 
            : base(coreWrapper) { }

        public override void OnPicked()
        {
            var activeUnitCoords = CoreWrapper.Core.currentStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates;
            var gridMapGenericDisplayData = new List<GridMapGenericDisplayData>()
            {
                new GridMapGenericDisplayData()
                {
                    Coordinates = activeUnitCoords,
                    Label = "(active)",
                }
            };
            GridMapPrinter.DefaultGridPrinter(CoreWrapper.Core.currentStateModel.BattleSceneState, gridMapGenericDisplayData).PrintMap();
        }
    }
}