using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands.Taunt;
using SMUBE.DataStructures.Units;

namespace SMUBE.Units
{
    public class UnitCommandProvider
    {
        private UnitData _unitData;
        public List<BaseCommand> AllCommands { get; }
        public List<BaseCommand> ViableCommands { get; } = new List<BaseCommand>();
        public List<UnitIdentifier> ViableTargets = new List<UnitIdentifier>();
        
        public bool IsTaunted { get; set; } = false;

        internal QLearningLastActionCacheData QLearningLastActionCache;

        internal class QLearningLastActionCacheData
        {
            public long stateId;
            public CommandId CommandId;
            public ArgsPreferences ArgsPreferences;
        }
        

        public UnitCommandProvider(UnitData unitData, List<BaseCommand> unitCommands)
        {
            AllCommands = unitCommands;
            _unitData = unitData;
        }

        public void OnNewTurn(BattleStateModel battleStateModel)
        {
            ViableCommands.Clear();
            ViableTargets.Clear();
            IsTaunted = false;
            
            FindBaseViableCommands(battleStateModel);
            ProcessPersistentEffects();
        }

        private void FindBaseViableCommands(BattleStateModel battleStateModel)
        {
            foreach (var command in AllCommands)
            {
                if (_unitData.UnitStats.CanUseAbility(command) 
                    && command.CommandArgsValidator.GetArgsPicker(command, battleStateModel).IsAnyValid())
                {
                    ViableCommands.Add(command);
                }
            }
        }

        private void ProcessPersistentEffects()
        {
            foreach (var effect in _unitData.UnitStats.PersistentEffects)
            {
                if (effect is TauntEffect tauntEffect)
                {
                    IsTaunted = true;
                    ViableTargets.Add(tauntEffect.Target);

                    ViableCommands.Clear();
                    ViableCommands.Add(new TauntedAttack());
                }
            }
        }
    }
}