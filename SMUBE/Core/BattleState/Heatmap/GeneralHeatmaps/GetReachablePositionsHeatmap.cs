using SMUBE.DataStructures.Units;

namespace SMUBE.BattleState.Heatmap.GeneralHeatmaps
{
    public class GetReachablePositionsHeatmap : BaseHeatmap
    {
        private readonly UnitIdentifier _unitIdentifier;
        protected override int PrefillValue => 0;

        public GetReachablePositionsHeatmap(UnitIdentifier unitIdentifier, BattleStateModel battleStateModel) : base(battleStateModel)
        {
            _unitIdentifier = unitIdentifier;
        }

        public override void ProcessHeatmap(BattleStateModel battleStateModel)
        {
            base.ProcessHeatmap(battleStateModel);

            var reachablePositions = battleStateModel.BattleSceneState.PathfindingHandler.AllUnitReachablePaths[_unitIdentifier];

            foreach (var reachablePosition in reachablePositions)
            {
                var coordinates = reachablePosition.TargetPosition.Coordinates;
                Set(coordinates.x, coordinates.y, 1);
            }
        }
    }
}