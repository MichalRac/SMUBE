using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneToEveryArgsPicker : ArgsPicker
    {
        public OneToEveryArgsPicker(ICommand command, BattleStateModel battleStateModel) : base(command, battleStateModel)
        {
        }

        public override void Submit()
        {
            var args = GetCommandArgs();
            IsResolved = true;
            ArgsConfirmed?.Invoke(args);
        }

        public override void Return()
        {
            OperationCancelled?.Invoke();
        }

        protected override void SetDefaultTarget()
        {
        }

        public override bool IsAnyValid()
        {
            return GetAllValidTargets().Count > 0;
        }

        public override CommandArgs GetCommandArgs()
        {
            var state = BattleStateModel;
            var args = new CommonArgs(state.ActiveUnit.UnitData, GetAllValidTargets(), state);
            return args;
        }

        private List<UnitData> GetAllValidTargets()
        {
            var result = new List<UnitData>();
            var constraint = Command.CommandArgsValidator.ArgsConstraint;
            var activeUnitTeamId = BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId;

            foreach (var unit in BattleStateModel.Units)
            {
                switch (constraint)
                {
                    case ArgsConstraint.None:
                        result.Add(unit.UnitData);
                        break;
                    case ArgsConstraint.Ally:
                        if (activeUnitTeamId == unit.UnitData.UnitIdentifier.TeamId)
                        {
                            result.Add(unit.UnitData);
                        }
                        break;
                    case ArgsConstraint.Enemy:
                        if (activeUnitTeamId != unit.UnitData.UnitIdentifier.TeamId)
                        {
                            result.Add(unit.UnitData);
                        }
                        break;
                    case ArgsConstraint.Position:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            return result;
        }

        public override void Up()
        {
        }

        public override void Down()
        {
        }

        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override string GetPickerInfo()
        {
            return "Confirm or cancel command on all targets";
        }

        public override string GetPickerState()
        {
            return "Confirm or cancel command on all targets";
        }

        public override CommandArgs GetSuggestedArgs(ArgsPreferences argsPreferences)
        {
            var unit = BattleStateModel.ActiveUnit;
            var activeUnitIdentifier = unit.UnitData.UnitIdentifier;
            var constraint = Command.CommandArgsValidator.ArgsConstraint;

            var viableTargets = new List<UnitData>();
            switch (constraint)
            {
                case ArgsConstraint.None:
                    add_ally_targets();
                    add_enemy_targets();
                    break;
                case ArgsConstraint.Ally:
                    add_ally_targets();
                    break;
                case ArgsConstraint.Enemy:
                    add_enemy_targets();
                    break;
            }

            return new CommonArgs(unit.UnitData, viableTargets, BattleStateModel);
        
            void add_ally_targets()
            {
                var targets = BattleStateModel.GetTeamUnits(activeUnitIdentifier.TeamId == 0 ? 0 : 1).Where(u => u.UnitData.UnitStats.IsAlive());
                foreach (var target in targets)
                {
                    viableTargets.Add(target.UnitData);
                }
            }

            void add_enemy_targets()
            {
                var targets = BattleStateModel.GetTeamUnits(activeUnitIdentifier.TeamId == 0 ? 1 : 0).Where(u => u.UnitData.UnitStats.IsAlive());
                foreach (var target in targets)
                {
                    viableTargets.Add(target.UnitData);
                }
            }
        }
    }
}