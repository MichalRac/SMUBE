﻿using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.Core;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using SMUBE.Units;

namespace SMUBE.AI.QLearning
{
    public class QLearningModel : AIModel
    {
        public const float WIN_GAME_REWARD = 5;
        public const float LOSE_GAME_PENALTY = -3;
        public const float ENEMY_DEFEATED_REWARD = 1;
        
        public float Alpha_LearningRate = 0.6f; // influence of feedback to current Q value, potentially to be gradually reduced
        public float Gamma_DiscountRate = 0.4f; // how much next state Q contributes (0.75 contributes around 0.05 to 10 steps before reward 1) 
        public float Rho_RandomRate = 0.2f; // chance of picking random action instead of best known
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
                Alpha_LearningRate = 0.6f;
                Gamma_DiscountRate = 0.4f;
                Rho_RandomRate = 0.2f;
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
            var opponentTeamCount = battleStateModel.GetAllTeamUnits(opponentTeamId).Count;
            
            var teamUnits = battleStateModel.GetAllTeamUnits(teamId);
            var currentTeamUnitStates = new List<long>();
            foreach (var teamUnit in teamUnits)
            {
                currentTeamUnitStates.Add(_qLearningState.GetStateNumber(battleStateModel, teamUnit));
            }
            
            var activeUnit = battleStateModel.ActiveUnit;
            var activeUnitStateId = _qLearningState.GetStateNumber(battleStateModel, activeUnit);
            var viableCommands = _qLearningCommandHelper.GetSubcommands(battleStateModel.ActiveUnit);
            
            BaseCommand commandToTake = null;
            if (learningEnabled && RngProvider.NextDouble() < Rho_RandomRate)
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

            float totalRewardForAll = -0;
            float totalRewardForActiveWithPenalty = -0.25f;

            var opponentPostActionDifference = opponentTeamCount - battleStateModel.GetTeamUnits(opponentTeamId).Count;
            if (opponentPostActionDifference > 0)
            {
                totalRewardForAll += opponentPostActionDifference * ENEMY_DEFEATED_REWARD;
            }

            if (battleStateModel.ActionsTakenCount > 5000)
            {
                totalRewardForActiveWithPenalty -= 2;
            }
            else if (battleStateModel.ActionsTakenCount > 2500)
            {
                totalRewardForActiveWithPenalty -= 1;
            }
            else if (battleStateModel.ActionsTakenCount > 1000)
            {
                totalRewardForActiveWithPenalty -= 0.5f;
            }
            else if (battleStateModel.ActionsTakenCount > 500)
            {
                totalRewardForActiveWithPenalty -= 0.25f;
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

                    var unitSpecificReward = teamUnit.UnitData.UnitIdentifier.Equals(activeUnitIdentifier)
                        ? totalRewardForAll + totalRewardForActiveWithPenalty
                        : totalRewardForAll;
                    
                    var newQValue = (1 - Alpha_LearningRate) * qValue + Alpha_LearningRate * (unitSpecificReward + Gamma_DiscountRate * nextMaxQValue);
                    
                    _qValueData.SetQValue(previousStateId, lastTakenAction, newQValue);
                }
            }

            return commandToTake;
        }

        public void OnGameEndCondition(BattleStateModel battleStateModel, Unit unit, int winnerTeamId)
        {
            if (!learningEnabled)
            {
                return;
            }

            var gameEndConditionReward = 0f;
            
            if (unit.UnitData.UnitIdentifier.TeamId == winnerTeamId)
            {
                gameEndConditionReward += WIN_GAME_REWARD;
            }
            else
            {
                gameEndConditionReward += LOSE_GAME_PENALTY;
            }
            
            if (unit.UnitCommandProvider.QLearningLastActionCache != null)
            {
                var qLearningLastActionCache = unit.UnitCommandProvider.QLearningLastActionCache;
                var previousStateId = qLearningLastActionCache.stateId;
                var newStateId = _qLearningState.GetStateNumber(battleStateModel, unit);

                var lastTakenAction = _qLearningCommandHelper.RetrieveCommand(qLearningLastActionCache.CommandId, qLearningLastActionCache.ArgsPreferences);
                var qValue = _qValueData.GetQValue(previousStateId, lastTakenAction);

                var allCommands = _qLearningCommandHelper.GetSubcommands(unit, false);
                var bestNextAction = _qValueData.GetBestViableCommandForState(newStateId, allCommands, learningEnabled);

                var nextMaxQValue = _qValueData.GetQValue(newStateId, bestNextAction);
                    
                var newQValue = (1 - Alpha_LearningRate) * qValue + Alpha_LearningRate * (gameEndConditionReward + Gamma_DiscountRate * nextMaxQValue);
                    
                _qValueData.SetQValue(previousStateId, lastTakenAction, newQValue);
            }

        }

        public override CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return command.GetSuggestedPseudoRandomArgs(battleStateModel);
        }
    }
}