using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.Core;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.AI.QLearning
{
    public class QLearningModel : AIModel
    {
        public const float WIN_GAME_REWARD = 5;
        public const float ENEMY_DEFEATED_REWARD = 1;
        
        public float Alpha_LearningRate = 0.7f; // influence of feedback to current Q value, potentially to be gradually reduced
        public float Gamma_DiscountRate = 0.85f; // how much next state Q contributes (0.75 contributes around 0.05 to 10 steps before reward 1) 
        public float Rho_RandomRate = 0.6f; // chance of picking random action instead of best known
        public float Nu_LenghtOfWalk = 0f; // chance of starting over from random state, can be 0 in this context

        public QValueData _qValueData = new QValueData();
        private QLearningCommandHelper _qLearningCommandHelper = new QLearningCommandHelper();
        private QLearningState _qLearningState = new QLearningState();

        private bool learningEnabled = true;
        
        public QLearningModel() : base(false)
        {
        }

        public QLearningModel(QValueData qTable) : base(false)
        {
            _qValueData = qTable;
            EnableLearning(false);
        }

        public void EnableLearning(bool value)
        {
            learningEnabled = value;
            if (value)
            {
                Alpha_LearningRate = 0.7f;
                Gamma_DiscountRate = 0.85f;
                Rho_RandomRate = 0.6f;
                Nu_LenghtOfWalk = 0f;
            }
            else
            {
                Alpha_LearningRate = 0f;
                Gamma_DiscountRate = 0f;
                Rho_RandomRate = 0f; 
                Nu_LenghtOfWalk = 0f;
            }
        }

        public override BaseCommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (RngProvider.NextDouble() < Nu_LenghtOfWalk)
            {
                // go to random state, not necessary since we run algorithm on series of full games
                throw new System.NotImplementedException();
            }

            var teamId = battleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId;
            var opponentTeamId = teamId == 0 ? 1 : 0;
            var opponentTeamCount = battleStateModel.GetTeamUnits(opponentTeamId).Count;
            
            var teamUnits = battleStateModel.GetTeamUnits(teamId);
            var currentTeamUnitStates = new List<long>();
            foreach (var teamUnit in teamUnits)
            {
                currentTeamUnitStates.Add(_qLearningState.GetStateNumber(battleStateModel, teamUnit));
            }
            
            var activeUnit = battleStateModel.ActiveUnit;
            var activeUnitStateId = _qLearningState.GetStateNumber(battleStateModel, activeUnit);
            var viableCommands = _qLearningCommandHelper.GetSubcommands(battleStateModel.ActiveUnit);
            
            BaseCommand commandToTake = null;
            if (RngProvider.NextDouble() < Rho_RandomRate)
            {
                viableCommands.Shuffle();
                commandToTake = viableCommands.First();
            }
            else
            {
                commandToTake = _qValueData.GetBestViableCommandForState(activeUnitStateId, viableCommands, learningEnabled);
            }

            var args = GetCommandArgs(commandToTake, battleStateModel, battleStateModel.ActiveUnit.UnitData.UnitIdentifier);

            battleStateModel.ExecuteCommand(commandToTake, args);

            if (!learningEnabled)
            {
                return commandToTake;
            }

            float totalReward = -0.25f;
            
            if (battleStateModel.IsFinished(out var winnerTeam))
            {
                if (winnerTeam == teamId)
                {
                    totalReward += WIN_GAME_REWARD;
                }
            }

            var opponentPostActionDifference = opponentTeamCount - battleStateModel.GetTeamUnits(opponentTeamId).Count;
            if (opponentPostActionDifference > 0)
            {
                totalReward += opponentPostActionDifference * ENEMY_DEFEATED_REWARD;
            }

            foreach (var teamUnit in teamUnits)
            {
                if (teamUnit.UnitCommandProvider.QLearningLastActionCache != null)
                {
                    var qLearningLastActionCache = teamUnit.UnitCommandProvider.QLearningLastActionCache;
                    var previousStateId = qLearningLastActionCache.stateId;
                    var newStateId = _qLearningState.GetStateNumber(battleStateModel, teamUnit);

                    var lastTakenAction = _qLearningCommandHelper.RetrieveCommand(qLearningLastActionCache.CommandId, qLearningLastActionCache.ArgsPreferences);
                    var qValue = _qValueData.GetQValue(previousStateId, lastTakenAction);

                    var allCommands = _qLearningCommandHelper.GetSubcommands(teamUnit, false);
                    var bestNextAction = _qValueData.GetBestViableCommandForState(newStateId, allCommands, learningEnabled);

                    var nextMaxQValue = _qValueData.GetQValue(newStateId, bestNextAction);
                    
                    var newQValue = (1 - Alpha_LearningRate) * qValue + Alpha_LearningRate * (totalReward + Gamma_DiscountRate * nextMaxQValue);
                    
                    _qValueData.SetQValue(previousStateId, lastTakenAction, newQValue);
                }
            }

            return commandToTake;
        }

        public override CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return command.GetSuggestedPseudoRandomArgs(battleStateModel);
        }
    }
}