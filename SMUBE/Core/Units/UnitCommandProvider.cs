using System.Collections.Generic;
using SMUBE.Commands;
using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands.Taunt;
using SMUBE.DataStructures.Units;

namespace SMUBE.Units
{
    public class UnitCommandProvider
    {
        private UnitData _unitData;
        public List<ICommand> AllCommands { get; }
        public List<ICommand> ViableCommands { get; } = new List<ICommand>();
        public List<UnitIdentifier> ViableTargets = new List<UnitIdentifier>();

        public bool IsTaunted { get; set; } = false;


        public UnitCommandProvider(UnitData unitData, List<ICommand> unitCommands)
        {
            AllCommands = unitCommands;
            _unitData = unitData;
        }

        public void OnNewTurn()
        {
            ViableCommands.Clear();
            ViableTargets.Clear();
            IsTaunted = false;
            
            FindBaseViableCommands();
            ProcessPersistentEffects();
        }

        private void FindBaseViableCommands()
        {
            foreach (var command in AllCommands)
            {
                if (_unitData.UnitStats.CanUseAbility(command))
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