using System;
using SMUBE.DataStructures.BattleScene;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerDisplayMap : InternalRunnerAction
    {
        public InternalRunnerDisplayMap(BattleCoreSimulationWrapper coreWrapper) : base(coreWrapper)
        {
        }

        public override void OnPicked()
        {
            var battleScene = CoreWrapper.Core.currentStateModel.BattleSceneState;
            var height = battleScene.Height;
            var width = battleScene.Width;
            var currentGrid = battleScene.Grid;

            for (int y = width - 1; y >= 0; y--)
            {
                for (int x = 0; x < height; x++)
                {
                        string contentString = string.Empty;
                        switch (currentGrid[x, x].ContentType)
                        {
                            case BattleScenePositionContentType.None:
                                contentString = "  [ ]";
                                break;
                            case BattleScenePositionContentType.Obstacle:
                                contentString = "  [X]";
                                break;
                            case BattleScenePositionContentType.Defensive:
                                contentString = " [!D!]";
                                break;
                            case BattleScenePositionContentType.Unstable:
                                contentString = " [!U!]";
                                break;
                        }

                        if (currentGrid[x, y].UnitIdentifier != null)
                        {
                            var id = $" [{currentGrid[x, y].UnitIdentifier.TeamId}:{currentGrid[x, y].UnitIdentifier.PersonalId}]";
                            contentString = id;
                        }

                        Console.Write($"{contentString}\t");

                }
                Console.Write("\n\n\n");
            }
        }
    }
}