namespace SMUBE.BattleState.Heatmap.GeneralHeatmaps
{
    public class InBetweenTeamsHeatmap : BaseHeatmap
    {
        private readonly DistanceToUnitOfTeamIdHeatmap _h0;
        private readonly DistanceToUnitOfTeamIdHeatmap _h1;

        public InBetweenTeamsHeatmap(BattleStateModel battleStateModel, DistanceToUnitOfTeamIdHeatmap h0, DistanceToUnitOfTeamIdHeatmap h1) 
            : base(battleStateModel)
        {
            _h0 = h0;
            _h1 = h1;
        }

        public override void ProcessHeatmap(BattleStateModel battleStateModel)
        {
            base.ProcessHeatmap(battleStateModel);

            for (int x = 0; x < battleStateModel.BattleSceneState.Width; x++)
            {
                for (int y = 0; y < battleStateModel.BattleSceneState.Height; y++)
                {
                    Heatmap[x][y] = _h0.Heatmap[x][y] - _h1.Heatmap[x][y];
                }
            }
        }
    }
}