using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.Commands.SpecificCommands.BaseAttack;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.Commands.SpecificCommands.BaseWalk;
using SMUBE.Commands.SpecificCommands.DefendAll;
using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.Commands.SpecificCommands.LowerEnemyDefense;
using SMUBE.Commands.SpecificCommands.RaiseObstacle;
using SMUBE.Commands.SpecificCommands.ShieldPosition;
using SMUBE.Commands.SpecificCommands.Tackle;
using SMUBE.Commands.SpecificCommands.Taunt;
using SMUBE.Commands.SpecificCommands.Teleport;
using SMUBE.Commands.SpecificCommands.Wait;
using SMUBE.DataStructures.Utils;

namespace SMUBE.AI.QLearning
{
    [Serializable]
    public class QValueData
    {
        private QLearningCommandHelper _qLearningCommandHelper = new QLearningCommandHelper();
        public ConcurrentDictionary<long, List<QValueActionPair>> QValueDataStorage = new ConcurrentDictionary<long, List<QValueActionPair>>();
        private object _storageLock = new object();

        public float GetQValue(long stateId, BaseCommand action)
        {
            lock (_storageLock)
            {
                InitializeActionPairsIfEmpty(stateId);
                
                if (QValueDataStorage.TryGetValue(stateId, out var qValueActionPairs))
                {
                    qValueActionPairs.Shuffle();
                    
                    foreach (var qValueActionPair in qValueActionPairs)
                    {
                        if (qValueActionPair.MatchesBaseCommand(action))
                        {
                            return qValueActionPair.QValue;
                        }
                    }
                }

                var newActionPair = new QValueActionPair(0f, action);
                QValueDataStorage.AddOrUpdate(stateId, 
                    l => new List<QValueActionPair>{newActionPair}, 
                    (l, list) => new List<QValueActionPair>{newActionPair});
                return newActionPair.QValue;
            }
        }

        public BaseCommand GetBestViableCommandForState(long stateId, List<BaseCommand> viableCommands, bool learningEnabled)
        {
            lock (_storageLock)
            {
                if (learningEnabled)
                {
                    InitializeActionPairsIfEmpty(stateId);
                }
                else if (!QValueDataStorage.ContainsKey(stateId))
                {
                    var allPossible = GetAllPossibleCommands();
                    allPossible.Shuffle();
                    foreach (var qValueActionPair in allPossible)
                    {
                        if(viableCommands.Any(viableCommand => viableCommand.CommandId == qValueActionPair.CommandId))
                        {
                            return _qLearningCommandHelper.RetrieveCommand(qValueActionPair.CommandId, qValueActionPair.ArgsPreferences);
                        }
                    } 
                }
            
                BaseCommand bestCommand = null;
                if (QValueDataStorage.TryGetValue(stateId, out var qValueActionPairs))
                {
                    qValueActionPairs.Shuffle();
                
                    var bestActionQValue = float.MinValue;
                    foreach (var qValueActionPair in qValueActionPairs)
                    {
                        if(viableCommands.Any(viableCommand => viableCommand.CommandId == qValueActionPair.CommandId))
                        {
                            if (qValueActionPair.QValue > bestActionQValue)
                            {
                                bestActionQValue = qValueActionPair.QValue;
                                bestCommand = _qLearningCommandHelper.RetrieveCommand(qValueActionPair.CommandId, qValueActionPair.ArgsPreferences);
                            }
                        }
                    }
                }
            
                return bestCommand;
            }
        }

        public void SetQValue(long stateId, BaseCommand action, float value)
        {
            lock (_storageLock)
            {
                InitializeActionPairsIfEmpty(stateId);
            
                if (QValueDataStorage.TryGetValue(stateId, out var qValueActionPairs))
                {
                    foreach (var qValueActionPair in qValueActionPairs)
                    {
                        if (qValueActionPair.MatchesBaseCommand(action))
                        {
                            // if already added, only update value
                            qValueActionPair.QValue = value;
                            return;
                        }
                    }
                }

                // if state not initialized or no matching command, throw exception (should be auto set up)
                throw new ArgumentException();
            }
        }
        
        
        private void InitializeActionPairsIfEmpty(long stateId)
        {
            if(QValueDataStorage.ContainsKey(stateId))
                return;
            
            var qValueActionPairs = GetAllPossibleCommands();
            
            QValueDataStorage.AddOrUpdate(stateId, 
                l => qValueActionPairs, 
                (l, list) => qValueActionPairs);
                /*(l, list) =>
                {
                    var current = QValueDataStorage[l];
                    var toAdd = qValueActionPairs;
                    var results = new System.Collections.Generic.List<QValueActionPair>();
                    foreach (var currentItem in current)
                    {
                        results.Add(currentItem);
                    }
                    foreach (var toAddItem in toAdd)
                    {
                        if (results.Any(item => item.Matches(toAddItem)))
                        {
                            continue;
                        }
                        results.Add(toAddItem);
                    }
                    return results;
                }*/
        }

        private static List<QValueActionPair> GetAllPossibleCommands()
        {
            return new List<QValueActionPair>
            {
                new QValueActionPair(0f, new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)0)),
                new QValueActionPair(0f, new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)1)),
                new QValueActionPair(0f, new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)2)),
                new QValueActionPair(0f, new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)3)),
                new QValueActionPair(0f, new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)4)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.None)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn)),
                new QValueActionPair(0f, new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn)),
                new QValueActionPair(0f, new BaseBlock()),
                new QValueActionPair(0f, new Wait()),
                new QValueActionPair(0f, new DefendAll()),
                new QValueActionPair(0f, new Taunt().WithPreferences(ArgsEnemyTargetingPreference.None)),
                new QValueActionPair(0f, new Taunt().WithPreferences(ArgsEnemyTargetingPreference.Closest)),
                new QValueActionPair(0f, new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints)),
                new QValueActionPair(0f, new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage)),
                new QValueActionPair(0f, new Taunt().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange)),
                new QValueActionPair(0f, new TauntedAttack()),
                new QValueActionPair(0f, new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None)),
                new QValueActionPair(0f, new Tackle().WithPreferences(ArgsEnemyTargetingPreference.Closest)),
                new QValueActionPair(0f, new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints)),
                new QValueActionPair(0f, new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage)),
                new QValueActionPair(0f, new Tackle().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange)),
                new QValueActionPair(0f, new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.None)),
                new QValueActionPair(0f, new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy)),
                new QValueActionPair(0f, new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenEnemies)),
                new QValueActionPair(0f, new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.None)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn)),
                new QValueActionPair(0f, new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn)),
                new QValueActionPair(0f, new Teleport().WithPreferences(ArgsMovementTargetingPreference.None)),
                new QValueActionPair(0f, new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach)),
                new QValueActionPair(0f, new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully)),
                new QValueActionPair(0f, new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively)),
                new QValueActionPair(0f, new Teleport().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition)),
                new QValueActionPair(0f, new HealAll()),
                new QValueActionPair(0f, new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.None)),
                new QValueActionPair(0f, new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly)),
                new QValueActionPair(0f, new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly)),
                new QValueActionPair(0f, new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy)),
                new QValueActionPair(0f, new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach)),
                new QValueActionPair(0f, new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams)),
                new QValueActionPair(0f, new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.None)),
                new QValueActionPair(0f, new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.Closest)),
                new QValueActionPair(0f, new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints)),
                new QValueActionPair(0f, new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage)),
                new QValueActionPair(0f, new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange))
            };
        }
    }
}